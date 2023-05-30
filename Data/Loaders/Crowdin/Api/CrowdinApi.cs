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
            var url = $"https://api.crowdin.com/api/v2/projects/{projectId}";
            var data = await GetFromApi<DataResponse<CrowdinProject>>(url);
            return data.Data;
        } 

        public async Task<IEnumerable<CrowdinDirectory>> GetDirectories(int projectId)
        {
            var url = $"https://api.crowdin.com/api/v2/projects/{projectId}/directories";
            var data = await GetFromApi<DataResponse<List<DataResponse<CrowdinDirectory>>>>(url);
            return data.Data.Select(f => f.Data);
        }

        public async Task<IEnumerable<CrowdinFile>> GetFiles(int projectId, int directoryId)
        {
            var url = $"https://api.crowdin.com/api/v2/projects/{projectId}/files?directoryId={directoryId}";
            var data = await GetFromApi<DataResponse<List<DataResponse<CrowdinFile>>>>(url);
            return data.Data.Select(f => f.Data);
        }

        public async Task<IEnumerable<CrowdinString>> GetStrings(int projectId, int fileId)
        {
            var url = $"https://api.crowdin.com/api/v2/projects/{projectId}/strings?fileId={fileId}";
            var data = await GetFromApi<DataResponse<List<DataResponse<CrowdinString>>>>(url);
            return data.Data.Select(f => f.Data);
        }

        public async Task<IEnumerable<CrowdinTranslation>> GetTranslations(int projectId, int fileId, string languageId)
        {
            var url = $"https://api.crowdin.com/api/v2/projects/{projectId}/languages/{languageId}/translations?fileId={fileId}";
            var data = await GetFromApi<DataResponse<List<DataResponse<CrowdinTranslation>>>>(url);
            return data.Data.Select(f => f.Data);
        }

        private async Task<T> GetFromApi<T>(string url)
        {
            using var httpClient = CreateHttpClient();
            var data = await httpClient.GetFromJsonAsync<T>(url);
            return data;
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
