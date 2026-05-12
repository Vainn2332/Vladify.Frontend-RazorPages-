using System.Net.Http.Headers;
using Vladify.Frontend.models;
using Vladify.Frontend.models.SongModels;

namespace Vladify.Frontend.services;

public class SongService(HttpClient client)
{
    public async Task<SongModel> CreateSongAsync(SongAddRequestModel songAddRequestModel, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(songAddRequestModel.Title), "title");
        content.Add(new StringContent(songAddRequestModel.Album), "album");
        content.Add(new StringContent(songAddRequestModel.Duration.ToString()), "duration");

        var audioStream = songAddRequestModel.AudioFile.OpenReadStream();
        var audioContent = new StreamContent(audioStream);
        content.Add(audioContent, "audioFile", songAddRequestModel.AudioFile.FileName);

        var imageStream = songAddRequestModel.Image.OpenReadStream();
        var imageContent = new StreamContent(imageStream);
        content.Add(imageContent, "image", songAddRequestModel.Image.FileName);


        var response = await client.PostAsync($"{MyConstants.BaseApiUrl}/api/songs", content, cancellationToken);

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

    public async Task<ICollection<SongModel>> GetRecentSongsAsync(PaginationFilter paginationFilter, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var songs = await client.GetFromJsonAsync<ICollection<SongModel>>($"{MyConstants.BaseApiUrl}/api/songs/recent?pageNumber={paginationFilter.PageNumber}&pageSize={paginationFilter.PageSize}");

        return songs!;
    }

    public async Task DeleteSongAsync(Guid songId, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.DeleteAsync($"{MyConstants.BaseApiUrl}/api/songs/{songId}", cancellationToken);

    }

}
