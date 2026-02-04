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
/// すべてのコマンドとクエリをバリデーションする。
/// </para>
/// </remarks>
internal static class ValidationDecorator
{
    /// <summary>
    /// レスポンスありコマンドハンドラー
    /// </summary>
    /// <typeparam name="TCommand">コマンド</typeparam>
    /// <typeparam name="TResponse">レスポンス</typeparam>
    /// <param name="innerHandler">内部ハンドラー</param>
    /// <param name="validators">バリデーター</param>
    /// <returns>レスポンス</returns>
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
    /// レスポンスなしコマンドハンドラー
    /// </summary>
    /// <typeparam name="TCommand">コマンド</typeparam>
    /// <param name="innerHandler">内部ハンドラー</param>
    /// <param name="validators">バリデーター</param>
    /// <returns>レスポンス</returns>
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
    /// バリデーションを実行する
    /// </summary>
    /// <typeparam name="TCommand">コマンド</typeparam>
    /// <param name="command">コマンド</param>
    /// <param name="validators">バリデーター</param>
    /// <returns>バリデーションエラー</returns>
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
    /// バリデーションエラーを作成する
    /// </summary>
    /// <param name="validationFailures">バリデーションエラー</param>
    /// <returns>バリデーションエラー</returns>
    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());
}
