using System.Net.Http.Headers;
using Vladify.Frontend.models;

namespace Vladify.Frontend.services;

public class SearchService(HttpClient client)
{
    public async Task<SearchResultDto> SearchAsync(string query, string accessToken, CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var result = await client.GetFromJsonAsync<SearchResultDto>($"{MyConstants.BaseApiUrl}/api/search?query={query}", cancellationToken);

        return result!;
    }
}
