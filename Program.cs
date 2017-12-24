using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var resId = "C62576FDA2F2250C!1418";
            var authKey = "";
            var fileName = Environment.CurrentDirectory + "/wrk.xlsx";

            IFileDownloader fileDownloader = new OneDriveFileDownloader(resId, authKey, fileName);

            ExcelConfiguration configuration = new ExcelConfiguration
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

            IFileDocumentReader fileDocumentReader = new ExcelFileDocumentReader(configuration);
            
            ISource source = new RemoteFileSource(fileDownloader, fileDocumentReader);

            var document = await source.LoadAsync();

            document.ToString();
        }
    }
}
