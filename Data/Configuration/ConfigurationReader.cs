using System;
using System.IO;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Configuration.Model;
using Newtonsoft.Json;

namespace AppLocalizationUtil.Data.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {
        public string FileName { get; set; }

        public ConfigurationReader(string fileName) 
        {
            FileName = fileName;
        }

        public async Task<ConfigurationModel> ReadAsync()
        {
            string fileName = $"{Environment.CurrentDirectory}/{FileName}";

            var configuration = JsonConvert.DeserializeObject<ConfigurationModel>(await File.ReadAllTextAsync(fileName));

            return configuration;
        }
    }
}