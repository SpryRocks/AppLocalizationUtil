using System;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Loaders;
using AppLocalizationUtil.Data.Sources;

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
            var authKey = "!ACv68v9KMDmOJLE";
            var fileName = Environment.CurrentDirectory + "/wrk.xlsx";

            IFileDownloader fileDownloader = new OneDriveFileDownloader(resId, authKey, fileName);
            IFileDocumentReader fileDocumentReader = new ExcelFileDocumentReader(new ExcelFileDocumentReader.Configuration());
            
            ISource source = new RemoteFileSource(fileDownloader, fileDocumentReader);

            var document = await source.LoadAsync();

            document.ToString();
        }
    }
}
