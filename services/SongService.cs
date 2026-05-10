using System.Net.Http.Headers;
using Vladify.Frontend.models;
using Vladify.Frontend.models.SongModels;

namespace Vladify.Frontend.services;

public class SongService(HttpClient client)
{
    public async Task<SongModel> CreateSongAsync(SongAddRequestModel songAddRequestModel, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.PostAsJsonAsync<SongAddRequestModel>($"{MyConstants.BaseApiUrl}/api/songs", songAddRequestModel);

        if (response.IsSuccessStatusCode)
        {
            var newSong = await response.Content.ReadFromJsonAsync<SongModel>();
            return newSong!;
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            throw new Exception($"{error?.ErrorTitle}\n{error?.ErrorMessage}");
        }
    }

    public async Task<ICollection<SongModel>> GetAllSongsOfUserAsync(Guid userId, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var songs = await client.GetFromJsonAsync<ICollection<SongModel>>($"{MyConstants.BaseApiUrl}/api/songs/user/{userId}");

        return songs!;
    }

    public async Task DeleteSongAsync(Guid songId, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.DeleteAsync($"{MyConstants.BaseApiUrl}/api/songs/{songId}", cancellationToken);

    }

}
