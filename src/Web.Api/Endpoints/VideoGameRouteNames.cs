namespace Web.Api.Endpoints;

/// <summary>
/// VideoGames機能のルート名定数を一元管理する。
/// </summary>
/// <remarks>
/// OpenAPI/Scalarドキュメント、CreatedAtRouteなどで使用。
/// With/Withoutの区別は入れ子クラスで整理する。
/// </remarks>
internal static class VideoGameRouteNames
{
    internal static class WithMediatR
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

    internal static class WithoutMediatR
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
}
