namespace Domain.VideoGame;

/// <summary>
/// ビデオゲームエンティティ
/// </summary>
/// <remarks>
/// <para>
/// データベースの VideoGames テーブルにマッピングされるドメインエンティティです。
/// </para>
/// <para>
/// <strong>検証ルール:</strong><br/>
/// すべてのプロパティには対応する検証ルールが <see cref="VideoGameValidationRules"/> に定義されています。
/// </para>
/// </remarks>
public class VideoGame // Entity
{
    /// <summary>
    /// ゲームの一意識別子（主キー）
    /// </summary>
    /// <remarks>
    /// データベースにより自動採番されます。
    /// </remarks>
    public int Id { get; set; }

    /// <summary>
    /// ゲームタイトル
    /// </summary>
    /// <remarks>
    /// <para>
    /// 必須項目です。最大 <see cref="VideoGameValidationRules.Title.MaxLength"/> 文字まで許可されます。
    /// </para>
    /// <para>
    /// <strong>例:</strong> "The Legend of Zelda: Breath of the Wild"
    /// </para>
    /// </remarks>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// ゲームジャンル
    /// </summary>
    /// <remarks>
    /// <para>
    /// 必須項目です。最大 <see cref="VideoGameValidationRules.Genre.MaxLength"/> 文字まで許可されます。
    /// </para>
    /// <para>
    /// <strong>一般的なジャンル例:</strong> "Action", "RPG", "Strategy", "Adventure", "Shooter"
    /// </para>
    /// </remarks>
    public string Genre { get; set; } = string.Empty;

    /// <summary>
    /// リリース年
    /// </summary>
    /// <remarks>
    /// <para>
    /// ゲームが公式にリリースされた年を表します。
    /// </para>
    /// <para>
    /// <strong>制約:</strong><br/>
    /// - 最小値: <see cref="VideoGameValidationRules.ReleaseYear.MinValue"/> (1950年)<br/>
    /// - 最大値: 現在の年
    /// </para>
    /// </remarks>
    public int ReleaseYear { get; set; }
}
