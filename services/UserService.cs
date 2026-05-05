using Vladify.Frontend.models;

namespace Vladify.Frontend.services;

public class UserService(HttpClient client)
{
    public async Task<UserModel> GetUserByEmailAsync(string email)
    {
        var user = await client.GetFromJsonAsync<UserModel>($"{MyConstants.BaseApiUrl}/{email}");

        return user!;
    }
}
