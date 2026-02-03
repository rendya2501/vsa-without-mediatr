using FluentValidation;
using MediatR;

namespace FeatureWithMediatR.Behaivors;

/// <summary>
/// MediatR の Pipeline Behavior。
/// Handler が呼ばれる前に FluentValidation を実行するための共通処理。
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="validators"></param>
/// <remarks>
/// DI から「TRequest に対応する Validator」をすべて受け取る
/// 通常は 0 個 or 1 個だが、複数あっても動く設計
/// </remarks>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    // MediatR のパイプラインに割り込むためのインターフェース
    : IPipelineBehavior<TRequest, TResponse>
    // TRequest は null 不可（ValidationContext が null を想定していないため）
    where TRequest : notnull
{
    /// <summary>
    /// MediatR が Request を処理するたびに必ず呼ばれるメソッド
    /// </summary>
    /// <param name="request">実際に送信された Command / Query</param>
    /// <param name="next">次の処理（次の Behavior or 最終的な Handler）</param>
    /// <param name="ct">キャンセル用トークン（ほぼ素通し）</param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (validators.Any())
        {
            // FluentValidation 用の検証コンテキストを作成
            var context = new ValidationContext<TRequest>(request);

            // 各 Validator で検証を実行
            var results = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, ct)));

            // すべての Validator を実行し、エラーを平坦化してリスト化
            var failures = results
                .SelectMany(r => r.Errors)          // 各 ValidationResult の Errors を 1 つの列にまとめる
                .ToList();

            // 1 件でもエラーがあれば Handler を呼ばずに例外を投げる
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next(ct);
    }
}
