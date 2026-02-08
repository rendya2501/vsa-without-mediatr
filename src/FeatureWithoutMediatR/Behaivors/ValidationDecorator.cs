using DomainKernel;
using FeatureShared.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace FeatureWithoutMediatR.Behaivors;

/// <summary>
/// バリデーションデコレーター
/// </summary>
/// <remarks>
/// <para>
/// Decorator パターンを使用して、すべての Command/Query Handler にバリデーション機能を追加します。
/// MediatR を使用しない軽量なアーキテクチャ向けの実装です。
/// </para>
/// <para>
/// <strong>MediatR 版との設計の違い:</strong><br/>
/// MediatR の ValidationBehavior は検証エラー時に ValidationException をスローしますが、
/// この Decorator 版は例外をスローせず、Result.Failure でエラーを返します。
/// これにより、例外を使用しないエラーハンドリングが可能になります。
/// </para>
/// <para>
/// <strong>動作フロー:</strong><br/>
/// 1. DI から対象の Command/Query に対応する IValidator&lt;T&gt; をすべて取得<br/>
/// 2. バリデーターが存在しない場合は即座に実際の Handler を実行<br/>
/// 3. バリデーターが存在する場合は並列実行し、すべての検証結果を集約<br/>
/// 4. 検証エラーがあれば Result.Failure を返す（Handler は実行されない）<br/>
/// 5. 検証成功時のみ実際の Handler を実行
/// </para>
/// <para>
/// <strong>複数バリデーターのサポート:</strong><br/>
/// 1つの Command に対して複数の Validator を登録できます。
/// すべての Validator が並列実行され、エラーは集約されます。
/// </para>
/// </remarks>
internal static class ValidationDecorator
{
    /// <summary>
    /// レスポンスありコマンドハンドラー用バリデーションデコレーター
    /// </summary>
    /// <typeparam name="TCommand">コマンド型（ICommand&lt;TResponse&gt; を実装）</typeparam>
    /// <typeparam name="TResponse">レスポンス型</typeparam>
    /// <param name="innerHandler">デコレート対象の実際のハンドラー</param>
    /// <param name="validators">DI コンテナから注入される FluentValidation のバリデーター群</param>
    /// <remarks>
    /// Result&lt;TResponse&gt; を返す Command 専用のデコレーターです。
    /// バリデーションエラー時は Result.Failure&lt;TResponse&gt; を返します。
    /// </remarks>
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
            : ICommandHandler<TCommand, TResponse>
            where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }

            return Result.Failure<TResponse>(CreateValidationError(validationFailures));
        }
    }

    /// <summary>
    /// レスポンスなしコマンドハンドラー用バリデーションデコレーター
    /// </summary>
    /// <typeparam name="TCommand">コマンド型（ICommand を実装）</typeparam>
    /// <param name="innerHandler">デコレート対象の実際のハンドラー</param>
    /// <param name="validators">DI コンテナから注入される FluentValidation のバリデーター群</param>
    /// <remarks>
    /// Result（レスポンスなし）を返す Command 専用のデコレーターです。
    /// バリデーションエラー時は Result.Failure を返します。
    /// </remarks>
    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
            : ICommandHandler<TCommand>
            where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }

            return Result.Failure(CreateValidationError(validationFailures));
        }
    }

    /// <summary>
    /// バリデーションを実行
    /// </summary>
    /// <typeparam name="TCommand">検証対象の Command 型</typeparam>
    /// <param name="command">検証対象の Command インスタンス</param>
    /// <param name="validators">適用するバリデーター群</param>
    /// <returns>検証エラーの配列（エラーがない場合は空配列）</returns>
    /// <remarks>
    /// 複数のバリデーターが存在する場合、Task.WhenAll で並列実行します。
    /// すべてのバリデーターの検証結果を集約し、失敗したものだけを抽出します。
    /// </remarks>
    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }

    /// <summary>
    /// バリデーションエラーを DomainKernel.ValidationError に変換
    /// </summary>
    /// <param name="validationFailures">FluentValidation の検証エラー群</param>
    /// <returns>DomainKernel の ValidationError インスタンス</returns>
    /// <remarks>
    /// FluentValidation の ValidationFailure を DomainKernel の Error 型に変換します。
    /// ErrorCode と ErrorMessage をそれぞれマッピングします。
    /// </remarks>
    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());
}
