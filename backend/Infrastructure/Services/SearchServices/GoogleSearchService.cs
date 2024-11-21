using Domain.Constants;
using Infrastructure.Interfaces;
using System.Text.RegularExpressions;

namespace Infrastructure.Services.SearchServices
{
    public class GoogleSearchService : ISearchService
    {
        private readonly HttpClient _httpClient;

        public GoogleSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", SearchConstants.UserAgent);
        }

        public async Task<string> SearchAsync(string keywords, string targetUrl)
        {
            var htmlContent = await FetchSearchHtmlAsync(keywords);

            var resultContainer = ExtractResultContainer(htmlContent);
            var anchorTags = ExtractAnchorTags(resultContainer);
            var matchedPositions = FindUrlPositions(anchorTags, targetUrl);

            return matchedPositions.Any() ? string.Join(", ", matchedPositions) : "0";
        }

        private async Task<string> FetchSearchHtmlAsync(string keywords)
        {
            var requestUrl = $"{SearchConstants.GoogleBaseUrl}/search?q={keywords}&num={SearchConstants.PageSize * SearchConstants.TotalPage}";
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private string ExtractResultContainer(string htmlContent)
        {
            const string startTag = @"<div[^>]*id=""rso""[^>]*>";
            const string openDivTag = @"<div[^>]*>";
            const string closeDivTag = @"<\/div>";

            var regexPattern = $@"{startTag}((?'nested'{openDivTag})|{closeDivTag}(?'-nested')|[\w\W]*?)*{closeDivTag}";
            var match = Regex.Match(htmlContent, regexPattern, RegexOptions.Singleline);

            return match.Success ? match.Value : string.Empty;
        }

        private List<string> ExtractAnchorTags(string resultContainer)
        {
            const string anchorPattern = @"<div[^>]*>([\s\S]*?)(<a[^>]*jsname\s*=\s*""[^""]+""[^>]*href\s*=\s*""[^""]+""[^>]*>[\s\S]*?<\/a>)[\s\S]*?<\/div>";

            var matches = Regex.Matches(resultContainer, anchorPattern, RegexOptions.Singleline);
            return matches.Select(match => match.Groups[2].Value).ToList();
        }

        private List<int> FindUrlPositions(List<string> anchorTags, string targetUrl)
        {
            var positions = new List<int>();

            for (int i = 0; i < anchorTags.Count && i < 100; i++)
            {
                if (anchorTags[i].Contains(targetUrl))
                {
                    positions.Add(i + 1);
                }
            }

            return positions;
        }
    }
}
