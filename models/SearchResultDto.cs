using Vladify.Frontend.models.UserModels;

namespace Vladify.Frontend.models;

public class SearchResultDto
{
    public ICollection<SongModel> Songs { get; set; } = [];
    public ICollection<UserModel> Users { get; set; } = [];
}
