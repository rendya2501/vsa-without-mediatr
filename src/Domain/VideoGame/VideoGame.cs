namespace Domain.VideoGame;

/// <summary>
/// ビデオゲームエンティティ
/// </summary>
public class VideoGame // Entity
{
    /// <summary>ID</summary>
    public int Id { get; set; }
    /// <summary>タイトル</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>ジャンル</summary>
    public string Genre { get; set; } = string.Empty;
    /// <summary>リリース年</summary>
    public int ReleaseYear { get; set; }
}
