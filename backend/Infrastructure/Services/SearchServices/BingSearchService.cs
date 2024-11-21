using Domain.Constants;
using Infrastructure.Interfaces;
using System.Text.RegularExpressions;

namespace Infrastructure.Services.SearchServices
{
    public class BingSearchService : ISearchService
    {
        private readonly HttpClient _httpClient;

        public BingSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", SearchConstants.UserAgent);
        }

        public async Task<string> SearchAsync(string keywords, string targetUrl)
        {
            var resultContents = await FetchSearchResultsAsync(keywords);

            var anchorTags = ExtractAnchorTags(resultContents);
            var matchingPositions = GetMatchingUrlPositions(anchorTags, targetUrl);

            return matchingPositions.Any()
                ? string.Join(", ", matchingPositions)
                : "0";
        }

        private async Task<List<string>> FetchSearchResultsAsync(string keywords)
        {
            var resultContents = new List<string>();

            for (int pageIndex = 0; pageIndex < SearchConstants.TotalPage; pageIndex++)
            {
                var htmlContent = await FetchPageHtmlAsync(keywords, pageIndex);
                var extractedContent = ExtractSearchResultContainer(htmlContent);

                if (!string.IsNullOrEmpty(extractedContent))
                {
                    resultContents.Add(extractedContent);
                }
            }

            return resultContents;
        }

        private async Task<string> FetchPageHtmlAsync(string keywords, int pageIndex)
        {
            var offset = pageIndex * SearchConstants.PageSize + 1;
            var url = $"{SearchConstants.BingBaseUrl}/search?q={keywords}&first={offset}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private string ExtractSearchResultContainer(string htmlContent)
        {
            const string pattern = @"<ol[^>]*id=""b_results""[^>]*>([\s\S]*?)<\/ol>";
            var match = Regex.Match(htmlContent, pattern, RegexOptions.Singleline);

            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private List<string> ExtractAnchorTags(IEnumerable<string> searchResultContents)
        {
            const string anchorPattern = @"<li[^>]*>([\s\S]*?)(<cite[^>]*>([\s\S]*?)<\/cite>)[\s\S]*?<\/li>";
            var anchors = new List<string>();

            foreach (var resultContent in searchResultContents)
            {
                var matches = Regex.Matches(resultContent, anchorPattern, RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    if (anchors.Count >= 100) break;
                    anchors.Add(match.Groups[2].Value);
                }
            }

            return anchors;
        }

        private List<int> GetMatchingUrlPositions(IEnumerable<string> anchorTags, string targetUrl)
        {
            var positions = new List<int>();
            var index = 1;

            foreach (var anchor in anchorTags)
            {
                if (anchor.Contains(targetUrl))
                {
                    positions.Add(index);
                }

                index++;
            }

            return positions;
        }
    }
}
