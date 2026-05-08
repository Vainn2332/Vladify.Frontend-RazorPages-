namespace Vladify.Frontend.models.PlaylistModels;

public class PlaylistModel
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string AuthorName { get; set; }

    public required ICollection<SongModel> Songs { get; set; }
}
