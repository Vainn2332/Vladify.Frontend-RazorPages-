using AutoMapper;
using Vladify.Frontend.models.UserModels;

namespace Vladify.Frontend.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserModel, UserUpdateRequestModel>();
    }
}
