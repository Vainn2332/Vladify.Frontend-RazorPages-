using System.Net.Http.Headers;
using Vladify.Frontend.models;
namespace Vladify.Frontend.services;

public class UserService(HttpClient client)
{
    public async Task<UserModel> GetCurrentUserAsync(string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var user = await client.GetFromJsonAsync<UserModel>($"{MyConstants.BaseApiUrl}/api/users");

        return user!;
    }
}
