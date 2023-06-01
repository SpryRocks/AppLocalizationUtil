using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Loaders.Crowdin.Models;

namespace AppLocalizationUtil.Data.Loaders.Crowdin.Api
{
    public class CrowdinApi
    {
        public string ApiKey { get; init; }

        public async Task<CrowdinProject> GetProject(int projectId)
        {
            var data = await GetFromApi<DataResponse<CrowdinProject>>(
                $"https://api.crowdin.com/api/v2/projects/{projectId}"
            );
            return data.Data;
        }

        public async Task<IEnumerable<CrowdinDirectory>> GetDirectories(int projectId)
        {
            var data = await GetFromApiUsingPagination<DataResponse<CrowdinDirectory>>(
                $"https://api.crowdin.com/api/v2/projects/{projectId}/directories"
            );
            return data.Data.Select(f => f.Data);
        }

        public async Task<IEnumerable<CrowdinFile>> GetFiles(int projectId, int directoryId)
        {
            var data = await GetFromApiUsingPagination<DataResponse<CrowdinFile>>(
                $"https://api.crowdin.com/api/v2/projects/{projectId}/files",
                new Dictionary<string, object>
                {
                    ["directoryId"] = directoryId,
                },
                50
            );
            return data.Data.Select(f => f.Data);
        }

        public async Task<IEnumerable<CrowdinString>> GetStrings(int projectId, int directoryId)
        {
            var data = await GetFromApiUsingPagination<DataResponse<CrowdinString>>(
                $"https://api.crowdin.com/api/v2/projects/{projectId}/strings",
                new Dictionary<string, object>
                {
                    ["directoryId"] = directoryId,
                },
                500
            );
            return data.Data.Select(f => f.Data);
        }

        public async Task<IEnumerable<CrowdinTranslation>> GetTranslations(int projectId, string languageId)
        {
            var data = await GetFromApiUsingPagination<DataResponse<CrowdinTranslation>>(
                $"https://api.crowdin.com/api/v2/projects/{projectId}/languages/{languageId}/translations",
                limit: 500
            );
            return data.Data.Select(f => f.Data);
        }

        private async Task<DataResponse<IEnumerable<T>>> GetFromApiUsingPagination<T>(string url,
            IDictionary<string, object> queryParams = null, int limit = 15)
        {
            using var httpClient = CreateHttpClient();

            var offset = 0;

            var results = new List<T>();

            int tmpCount;
            do
            {
                var tmpQueryParams = new Dictionary<string, object>(queryParams ?? new Dictionary<string, object>())
                {
                    ["limit"] = limit,
                    ["offset"] = offset,
                };

                var tmpUrl = PrepareUrl(url, tmpQueryParams);
                var tmpResults = await httpClient.GetFromJsonAsync<DataResponse<List<T>>>(tmpUrl);
                tmpCount = tmpResults.Data.Count;
                results.AddRange(tmpResults.Data);
                offset += tmpCount;
            } while (!(tmpCount < limit));

            return new DataResponse<IEnumerable<T>> {Data = results};
        }

        private async Task<T> GetFromApi<T>(string url, IDictionary<string, object> queryParams = null)
        {
            using var httpClient = CreateHttpClient();
            return await httpClient.GetFromJsonAsync<T>(PrepareUrl(url, queryParams));
        }

        private string PrepareUrl(string url, IDictionary<string, object> queryParams)
        {
            if (queryParams == null) return url;

            var queryParts = queryParams.Select(it => $"{it.Key}={it.Value}");
            var queryString = string.Join("&", queryParts);
            url += $"?{queryString}";

            return url;
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            if (ApiKey == null)
            {
                throw new Exception("Crowdin ApiKey not set");
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
            return httpClient;
        }
    }
}