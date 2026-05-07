namespace Vladify.Frontend.models.UserModels;

public class UserUpdateRequestModel
{
    public required string Name { get; set; }

    public required int Age { get; set; }

    public required Gender Gender { get; set; }
}
