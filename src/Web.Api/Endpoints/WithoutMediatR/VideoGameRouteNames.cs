namespace Web.Api.Endpoints.WithoutMediatR;

/// <summary>
/// ルート名定数
/// </summary>
/// <remarks>
/// OpenAPI/Scalarドキュメント、CreatedAtRouteなどで使用。
/// ルート名を変更する場合はここを修正するだけで全体に反映される。
/// </remarks>
internal static class VideoGameRouteNames
{
    /// <summary>全ゲーム一覧取得</summary>
    public const string GetAll = "GetAllGames2";

    /// <summary>ID指定ゲーム取得</summary>
    public const string GetById = "GetGameById2";

    /// <summary>ゲーム作成</summary>
    public const string Create = "CreateGame2";

    /// <summary>ゲーム更新</summary>
    public const string Update = "UpdateGame2";

    /// <summary>ゲーム削除</summary>
    public const string Delete = "DeleteGame2";
}
