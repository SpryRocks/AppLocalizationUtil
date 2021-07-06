using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppLocalizationUtil.Data.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {
        public static IConfigurationReader Create(string filename)
        {
            return new ConfigurationReader(filename);
        }
        
        public string FileName { get; set; }

        private ConfigurationReader(string fileName) 
        {
            FileName = fileName;
        }

        public async Task<ConfigurationModel> ReadAsync()
        {
            Console.WriteLine($"Reading... [{FileName}]");

            var configuration = JsonConvert.DeserializeObject<ConfigurationModel>(await File.ReadAllTextAsync(FileName));

            return configuration;
        }
    }
}