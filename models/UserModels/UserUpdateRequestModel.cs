using System.ComponentModel.DataAnnotations;

namespace Vladify.Frontend.models.UserModels;

public class UserUpdateRequestModel
{
    public required string Name { get; set; }

    public required int Age { get; set; }

    [Range(1, 2, ErrorMessage = "Выберите пол")]
    public required Gender Gender { get; set; }
}
