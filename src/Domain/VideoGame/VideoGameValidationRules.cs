namespace Domain.VideoGame;

/// <summary>
/// ビデオゲームバリデーションルール定数
/// </summary>
public static class VideoGameValidationRules
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
