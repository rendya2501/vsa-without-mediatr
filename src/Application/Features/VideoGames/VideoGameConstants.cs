namespace Application.Features.VideoGames;

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
public static class VideoGameConstants
{
    // ===================================================================
    // バリデーションルール
    // ===================================================================

    /// <summary>
    /// バリデーションルール定数
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// タイトル関連のバリデーション定数
        /// </summary>
        public static class Title
        {
            /// <summary>タイトルの最大文字数</summary>
            public const int MaxLength = 100;
        }

        /// <summary>
        /// ジャンル関連のバリデーション定数
        /// </summary>
        public static class Genre
        {
            /// <summary>ジャンルの最大文字数</summary>
            public const int MaxLength = 50;
        }

        /// <summary>
        /// リリース年関連のバリデーション定数
        /// </summary>
        public static class ReleaseYear
        {
            /// <summary>
            /// リリース年の最小値（1950年）
            /// </summary>
            /// <remarks>
            /// 1950年は商業的なビデオゲームが登場した時期の近似値。
            /// <list type="bullet">
            /// <item>1952年: OXO (Noughts and Crosses) - 最初期のビデオゲーム</item>
            /// <item>1958年: Tennis for Two</item>
            /// <item>1962年: Spacewar!</item>
            /// </list>
            /// 1950年以前のゲームは学術的・実験的なものが多く、
            /// 商業データベースでの管理対象外とする。
            /// </remarks>
            public const int MinValue = 1950;

            /// <summary>デフォルトのリリース年</summary>
            public const int DefaultValue = MinValue;
        }
    }

    // ===================================================================
    // ルート名
    // ===================================================================

    /// <summary>
    /// ルート名定数
    /// </summary>
    /// <remarks>
    /// OpenAPI/Scalarドキュメント、CreatedAtRouteなどで使用。
    /// ルート名を変更する場合はここを修正するだけで全体に反映される。
    /// </remarks>
    public static class RouteNames
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
}
