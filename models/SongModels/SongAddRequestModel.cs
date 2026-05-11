namespace Vladify.Frontend.models.SongModels;

public class SongAddRequestModel
{
    public required string Title { get; set; }

    public required string Album { get; set; }

    public TimeSpan Duration { get; set; }

    public required IFormFile AudioFile { get; set; }

    public required IFormFile Image { get; set; }
}
