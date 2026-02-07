using DomainKernel;
using FeatureShared.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace FeatureShared.Extensions;

/// <summary>
/// ResultをHTTPレスポンスに変換する拡張メソッド
/// </summary>
public static class ResultHttpExtensions
{
    /// <summary>
    /// 200 OK
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static IResult ToOk<T>(this Result<T> result)
        => result.Match(Results.Ok, CustomResults.Problem);

    /// <summary>
    /// 201 Created At Route
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="routeName"></param>
    /// <param name="routeValuesSelector"></param>
    /// <returns></returns>
    public static IResult ToCreatedAtRoute<T>(
        this Result<T> result,
        string routeName,
        Func<T, object> routeValuesSelector)
        => result.Match(
            value => Results.CreatedAtRoute(routeName, routeValuesSelector(value), value),
            CustomResults.Problem);

    /// <summary>
    /// 205 No Content
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static IResult ToNoContent(this Result result)
        => result.Match(Results.NoContent, CustomResults.Problem);

    /// <summary>
    /// カスタムレスポンス
    /// </summary>
    /// <typeparam name="T">返り値の型</typeparam>
    /// <param name="result">結果オブジェクト</param>
    /// <param name="onSuccess">成功時の処理</param>
    /// <returns>IResult オブジェクト</returns>
    /// <remarks>
    /// <para><strong>202 Accepted 例</strong></para>
    /// <code>
    /// public static async Task&lt;IResult&gt; Endpoint(...)
    ///     => (await sender.Send(new StartJobCommand(id), ct))
    ///         .ToResult(job => Results.Accepted($"/api/jobs/{job.Id}", job));
    /// </code>
    /// </remarks>
    public static IResult ToResult<T>(
        this Result<T> result,
        Func<T, IResult> onSuccess)
            => result.Match(onSuccess, CustomResults.Problem);

    /// <summary>
    /// カスタムレスポンス
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccess"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para><strong>複雑なロジック 例</strong></para>
    /// <code>
    /// public static async Task&lt;IResult&gt; Endpoint(ISender sender, int id, CancellationToken ct)
    ///     => (await sender.Send(new SomeCommand(id), ct))
    ///         .ToResult(value =>
    ///         {
    ///             // 複雑なロジック
    ///             var headers = new Dictionary&lt;string, string&gt;
    ///             {
    ///                 ["X-Custom-Header"] = value.SomeProperty
    ///             };
    ///             return Results.Ok(value);
    ///         });
    /// </code>
    /// </remarks>
    public static IResult ToResult(
        this Result result,
        Func<IResult> onSuccess)
            => result.Match(onSuccess, CustomResults.Problem);
}
