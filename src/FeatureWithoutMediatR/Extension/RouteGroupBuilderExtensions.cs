using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FeatureWithoutMediatR.Extension;

/// <summary>
/// RouteGroupBuilderの拡張メソッド
/// </summary>
public static class RouteGroupBuilderExtensions
{
    /// <summary>
    /// API グループを作成し、タグを設定する
    /// </summary>
    /// <param name="app">エンドポイントルートビルダー</param>
    /// <param name="prefix">APIのプレフィックス (例: "/api/games")</param>
    /// <param name="tag">OpenAPI タグ名</param>
    /// <returns>設定済みの RouteGroupBuilder</returns>
    public static RouteGroupBuilder MapApiGroup(
        this IEndpointRouteBuilder app,
        string prefix,
        string tag)
    {
        return app.MapGroup(prefix)
            .WithTags(tag);
    }

    /// <summary>
    /// API グループを作成し、複数のタグを設定する
    /// </summary>
    /// <param name="app">エンドポイントルートビルダー</param>
    /// <param name="prefix">APIのプレフィックス (例: "/api/games")</param>
    /// <param name="tags">OpenAPI タグ名の配列</param>
    /// <returns>設定済みの RouteGroupBuilder</returns>
    public static RouteGroupBuilder MapApiGroup(
        this IEndpointRouteBuilder app,
        string prefix,
        params string[] tags)
    {
        return app.MapGroup(prefix)
            .WithTags(tags);
    }
}
