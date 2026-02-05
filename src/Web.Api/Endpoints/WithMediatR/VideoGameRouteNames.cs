namespace Web.Api.Endpoints.WithMediatR;

/// <summary>
/// VideoGames機能のすべての定数定義
/// </summary>
/// <remarks>
/// <para>
/// バリデーションルール、ルート名、その他の定数を一元管理。
/// ネストクラスを使用して、関連する定数をグループ化。
/// </para>
/// <para>
/// <strong>構造:</strong><br/>
/// - Validation: バリデーションルール関連<br/>
/// - RouteNames: エンドポイントのルート名<br/>
/// - (将来の拡張): Paths, CacheKeys, ErrorMessages など
/// </para>
/// </remarks>
internal static class VideoGameRouteNames
{
    /// <summary>全ゲーム一覧取得</summary>
    public const string GetAll = "GetAllGames";
    /// <summary>ID指定ゲーム取得</summary>
    public const string GetById = "GetGameById";
    /// <summary>ゲーム作成</summary>
    public const string Create = "CreateGame";
    /// <summary>ゲーム更新</summary>
    public const string Update = "UpdateGame";
    /// <summary>ゲーム削除</summary>
    public const string Delete = "DeleteGame";
}
