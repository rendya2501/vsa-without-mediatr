using DomainKernel;
using Web.Api.Extensions;

namespace FeatureShared.Extensions;

/// <summary>
/// <see cref="Result"/> および <see cref="Result{TValue}"/> を HTTP レスポンスに変換する拡張メソッド
/// </summary>
/// <remarks>
/// <para>
/// 成功時は対応する HTTP ステータスコードのレスポンスを返し、
/// 失敗時は <see cref="ErrorToProblemMapper"/> を通じて RFC 7807 準拠の ProblemDetails に変換します。
/// </para>
/// <para>
/// <strong>使い分けの指針:</strong><br/>
/// - レスポンスパターンが1通りに決まる場合 → <see cref="ToOk{T}"/>、<see cref="ToNoContent"/> を使用<br/>
/// - ルート名や追加パラメータが必要な場合、または複雑なレスポンスを返す場合 → <see cref="ToResult{T}"/>、<see cref="ToResult"/> を使用
/// </para>
/// </remarks>
public static class ResultHttpExtensions
{
    /// <summary>
    /// 成功時に 200 OK、失敗時に ProblemDetails を返します
    /// </summary>
    /// <typeparam name="T">成功時の値の型</typeparam>
    /// <param name="result">変換対象の Result</param>
    /// <returns>200 OK または ProblemDetails を含む <see cref="IResult"/></returns>
    public static IResult ToOk<T>(this Result<T> result) =>
        result.Match(
            Results.Ok,
            failure => ErrorToProblemMapper.ToHttpResult(failure.Error!));

    /// <summary>
    /// 成功時に 204 No Content、失敗時に ProblemDetails を返します
    /// </summary>
    /// <param name="result">変換対象の Result</param>
    /// <returns>204 No Content または ProblemDetails を含む <see cref="IResult"/></returns>
    public static IResult ToNoContent(this Result result) =>
        result.Match(
            Results.NoContent,
            failure => ErrorToProblemMapper.ToHttpResult(failure.Error!));

    /// <summary>
    /// 成功時に <paramref name="onSuccess"/> の結果、失敗時に ProblemDetails を返します
    /// </summary>
    /// <typeparam name="T">成功時の値の型</typeparam>
    /// <param name="result">変換対象の Result</param>
    /// <param name="onSuccess">成功時に実行するレスポンス生成処理</param>
    /// <returns><paramref name="onSuccess"/> の結果または ProblemDetails を含む <see cref="IResult"/></returns>
    /// <remarks>
    /// <para>
    /// <see cref="ToOk{T}"/> や <see cref="ToNoContent"/> で対応できない場合に使用します。
    /// 201 Created や 202 Accepted など、呼び出し側でレスポンスを組み立てるケースが対象です。
    /// </para>
    /// <example>
    /// <strong>201 Created の例</strong>
    /// <code>
    /// return result.ToResult(response =>
    ///     Results.CreatedAtRoute(
    ///         VideoGameRouteNames.WithMediatR.GetById,
    ///         new { id = response.Id },
    ///         response));
    /// </code>
    /// </example>
    /// <example>
    /// <strong>202 Accepted の例</strong>
    /// <code>
    /// return result.ToResult(job =>
    ///     Results.Accepted($"/api/jobs/{job.Id}", job));
    /// </code>
    /// </example>
    /// </remarks>
    public static IResult ToResult<T>(this Result<T> result, Func<T, IResult> onSuccess) =>
        result.Match(
            onSuccess,
            failure => ErrorToProblemMapper.ToHttpResult(failure.Error!));

    /// <summary>
    /// 成功時に <paramref name="onSuccess"/> の結果、失敗時に ProblemDetails を返します（値なし版）
    /// </summary>
    /// <param name="result">変換対象の Result</param>
    /// <param name="onSuccess">成功時に実行するレスポンス生成処理</param>
    /// <returns><paramref name="onSuccess"/> の結果または ProblemDetails を含む <see cref="IResult"/></returns>
    /// <remarks>
    /// <para>
    /// 値を持たない <see cref="Result"/> に対して、カスタムレスポンスを返したい場合に使用します。
    /// </para>
    /// <example>
    /// <strong>カスタムヘッダーを含むレスポンスの例</strong>
    /// <code>
    /// return result.ToResult(() =>
    /// {
    ///     httpContext.Response.Headers["X-Custom-Header"] = "value";
    ///     return Results.Ok();
    /// });
    /// </code>
    /// </example>
    /// </remarks>
    public static IResult ToResult(this Result result, Func<IResult> onSuccess) =>
        result.Match(
            onSuccess,
            failure => ErrorToProblemMapper.ToHttpResult(failure.Error!));
}
