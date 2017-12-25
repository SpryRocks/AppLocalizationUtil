using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Configuration;
using AppLocalizationUtil.Data.Loaders;
using AppLocalizationUtil.Data.Sources;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            IConfigurationReader reader = new ConfigurationReader("Config.json");

            var configuration = await reader.ReadAsync();
            var placementConfig = configuration.Source["Placement"]["Config"];

            var resId = placementConfig.Value<string>("ResId");
            var authKey = placementConfig.Value<string>("AuthKey");
            
            var fileName = Environment.CurrentDirectory + "/wrk.xlsx";

            IFileDownloader fileDownloader = new OneDriveFileDownloader(resId, authKey, fileName);

            ExcelConfiguration excelConfiguration = new ExcelConfiguration
            {
                LanguageColumns = new Dictionary<string, Language>
                {
                    { "English (default)", new Language { Id = "en", Name = "English" } },
                    { "Dutch", new Language { Id = "nl", Name = "Dutch" } }
                },
                PlatformKeyColumns = new Dictionary<string, string>
                {
                    { "Key (Android)", "Android" },
                    { "Key (Web)", "Web" }
                },
                AppsColumn = "App"
            };

            IFileDocumentReader fileDocumentReader = new ExcelFileDocumentReader(excelConfiguration);
            
            ISource source = new RemoteFileSource(fileDownloader, fileDocumentReader);

            var document = await source.LoadAsync();

            document.ToString();
        }
    }
}
