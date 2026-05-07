using System.Net.Http.Headers;
using Vladify.Frontend.models;

namespace Vladify.Frontend.services;

public class PlaylistService(HttpClient client)
{
    public async Task<IEnumerable<PlaylistModel>> GetPlaylistsOfCurrentUserAsync(PaginationFilter paginationFilter, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var playlists = await client.GetFromJsonAsync<IEnumerable<PlaylistModel>>($"{MyConstants.BaseApiUrl}/api/playlists?pageNumber={paginationFilter.PageNumber}&pageSize={paginationFilter.PageSize}", cancellationToken);

        return playlists;
    }
}
