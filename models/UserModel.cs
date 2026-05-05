namespace Vladify.Frontend.models;

public class UserModel
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Album { get; set; }

    public required string Author { get; set; }

    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
}
