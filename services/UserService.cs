using System.Net.Http.Headers;
using Vladify.Frontend.models;
using Vladify.Frontend.models.UserModels;
namespace Vladify.Frontend.services;

public class UserService(HttpClient client)
{
    public async Task<UserModel> GetCurrentUserAsync(string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var user = await client.GetFromJsonAsync<UserModel>($"{MyConstants.BaseApiUrl}/api/users/currentUser", cancellationToken);

        return user!;
    }

    public async Task<UserModel> UpdateUserAsync(UserUpdateRequestModel userUpdateRequestModel, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.PutAsJsonAsync<UserUpdateRequestModel>($"{MyConstants.BaseApiUrl}/api/users/currentUser", userUpdateRequestModel, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var updatedUser = await response.Content.ReadFromJsonAsync<UserModel>();
            return updatedUser!;
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            throw new Exception($"{error?.ErrorTitle}\n{error?.ErrorMessage}");
        }
    }
}
