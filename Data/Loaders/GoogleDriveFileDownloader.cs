using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppLocalizationUtil.Data.Loaders
{
    public class GoogleDriveFileDownloader : IFileDownloader
    {
        private readonly string _id;
        public string FileName { get; set; }

        public GoogleDriveFileDownloader(string id)
        {
            _id = id;
        }

        public GoogleDriveFileDownloader(string id, string fileName) : this(id)
        {
            FileName = fileName;
        }

        public async Task DownloadAsync()
        {
            Console.WriteLine($"Download file from Google Drive... [Id: ***]");
            
            using (var client = new HttpClient())
            {
                var url = $"https://docs.google.com/uc?export=download&id={_id}";
                
                using (var result = await client.GetStreamAsync(url))
                {
                    using (var file = new FileStream(FileName, FileMode.Create))
                    {
                        await result.CopyToAsync(file);
                    }
                }
            }
        }
    }
}