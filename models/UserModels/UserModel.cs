using System.ComponentModel.DataAnnotations;

namespace Vladify.Frontend.models.UserModels;

public class UserModel
{
    public Guid Id { get; set; }

    public required string ExternalId { get; set; }

    public required string Name { get; set; }

    public required string EmailAddress { get; set; }

    public required int Age { get; set; }

    [Range(1, 2, ErrorMessage = "Выберите пол")]
    public Gender Gender { get; set; } = Gender.Undefined;
}
