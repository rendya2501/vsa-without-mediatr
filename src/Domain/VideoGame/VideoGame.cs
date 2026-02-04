namespace Domain.VideoGame;

public class VideoGame // Entity
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Genre { get; set; }
    public required int ReleaseYear { get; set; }
}
