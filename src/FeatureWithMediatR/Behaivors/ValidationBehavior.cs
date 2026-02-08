using DomainKernel;
using FluentValidation;
using MediatR;

namespace FeatureWithMediatR.Behaivors;

/// <summary>
/// バリデーションビヘイビアー
/// </summary>
/// <typeparam name="TRequest">MediatR の Request 型（Command または Query）</typeparam>
/// <typeparam name="TResponse">MediatR の Response 型（Result を継承）</typeparam>
/// <param name="validators">DI コンテナから注入される FluentValidation のバリデーター群</param>
/// <remarks>
/// <para>
/// MediatR の IPipelineBehavior を実装し、Handler 実行前に FluentValidation による検証を行います。
/// </para>
/// <para>
/// <strong>動作フロー:</strong><br/>
/// 1. DI から TRequest に対応するすべての IValidator&lt;TRequest&gt; を受け取る<br/>
/// 2. バリデーターが存在しない場合は即座に次の処理へ進む<br/>
/// 3. バリデーターが存在する場合は並列実行し、すべての検証結果を集約<br/>
/// 4. 検証エラーがあれば FluentValidation.ValidationException をスロー（Handler は実行されない）<br/>
/// 5. 検証成功時のみ Handler を実行
/// </para>
/// <para>
/// <strong>複数バリデーターのサポート:</strong><br/>
/// 通常、1つの Request に対するバリデーターは1つですが、
/// 複数登録されている場合でも正しく動作します。
/// </para>
/// </remarks>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    /// <summary>
    /// MediatR パイプラインでの処理実行
    /// </summary>
    /// <param name="request">検証対象の Command または Query</param>
    /// <param name="next">次の処理（LoggingBehavior → Handler）</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns>Handler からの Response（検証成功時のみ）</returns>
    /// <exception cref="ValidationException">検証エラーが1件以上存在する場合</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // バリデーターを配列化して1回だけ列挙し、バリデーターがなければ次のハンドラーを実行
        var validatorArray = validators as IValidator<TRequest>[] ?? validators.ToArray();

        // バリデーターがなければ次のハンドラーを実行
        if (validatorArray.Length == 0)
        {
            return await next(cancellationToken);
        }

        // FluentValidation 用の検証コンテキストを作成
        var context = new ValidationContext<TRequest>(request);

        // 各バリデーターで検証を実行
        var results = await Task.WhenAll(
            validatorArray.Select(v => v.ValidateAsync(context, cancellationToken)));

        // すべての Validator を実行し、エラーを平坦化してリスト化
        var failures = results
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        // 1 件でもエラーがあれば Handler を呼ばずに例外を投げる
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}
