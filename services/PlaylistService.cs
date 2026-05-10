using System.Net.Http.Headers;
using Vladify.Frontend.models;
using Vladify.Frontend.models.PlaylistModels;

namespace Vladify.Frontend.services;

public class PlaylistService(HttpClient client)
{
    public async Task<PlaylistModel> AddSongToPlaylistAsync(Guid songId, Guid playlistId, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.PostAsync($"{MyConstants.BaseApiUrl}/api/playlists/{playlistId}/songs/{songId}", null, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var newPlaylist = await response.Content.ReadFromJsonAsync<PlaylistModel>(cancellationToken);
            return newPlaylist!;
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>(cancellationToken);
            throw new Exception($"{error?.ErrorTitle}\n{error?.ErrorMessage}");
        }
    }

    public async Task<IEnumerable<PlaylistModel>> GetPlaylistsOfCurrentUserAsync(PaginationFilter paginationFilter, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var playlists = await client.GetFromJsonAsync<IEnumerable<PlaylistModel>>($"{MyConstants.BaseApiUrl}/api/playlists?pageNumber={paginationFilter.PageNumber}&pageSize={paginationFilter.PageSize}", cancellationToken);

        return playlists;
    }

    public async Task<PlaylistModel> GetPlaylistByIdAsync(Guid playlistId, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var playlist = await client.GetFromJsonAsync<PlaylistModel>($"{MyConstants.BaseApiUrl}/api/playlists/{playlistId}", cancellationToken);

        return playlist;
    }

    public async Task<PlaylistModel> AddNewPlaylistAsync(PlaylistAddRequestModel playlistAddRequestModel, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.PostAsJsonAsync($"{MyConstants.BaseApiUrl}/api/playlists", playlistAddRequestModel, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var newPlaylist = await response.Content.ReadFromJsonAsync<PlaylistModel>(cancellationToken);
            return newPlaylist!;
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>(cancellationToken);
            throw new Exception($"{error?.ErrorTitle}\n{error?.ErrorMessage}");
        }
    }

    public async Task<PlaylistModel> UpdatePlaylistAsync(PlaylistUpdateRequestModel playlistUpdateRequestModel, Guid playlistId, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.PutAsJsonAsync($"{MyConstants.BaseApiUrl}/api/playlists/{playlistId}", playlistUpdateRequestModel, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var newPlaylist = await response.Content.ReadFromJsonAsync<PlaylistModel>(cancellationToken);
            return newPlaylist!;
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>(cancellationToken);
            throw new Exception($"{error?.ErrorTitle}\n{error?.ErrorMessage}");
        }
    }

    public async Task<PlaylistModel> DeleteSongFromPlaylistAsync(Guid playlistId, Guid songId, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.DeleteAsync($"{MyConstants.BaseApiUrl}/api/playlists/{playlistId}/songs/{songId}", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var playlist = await response.Content.ReadFromJsonAsync<PlaylistModel>(cancellationToken);
            return playlist!;
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>(cancellationToken);
            throw new Exception($"{error?.ErrorTitle}\n{error?.ErrorMessage}");
        }

    }

    public async Task DeletePlaylistAsync(Guid playlistId, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.DeleteAsync($"{MyConstants.BaseApiUrl}/api/playlists/{playlistId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>(cancellationToken);
            throw new Exception($"{error?.ErrorTitle}\n{error?.ErrorMessage}");
        }
    }
}
