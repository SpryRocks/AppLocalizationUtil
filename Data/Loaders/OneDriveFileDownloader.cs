using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppLocalizationUtil.Data.Loaders
{
    public class OneDriveFileDownloader : IFileDownloader
    {
        private readonly string _resId;
        private readonly string _authKey;
        public string FileName { get; set; }

        public OneDriveFileDownloader(string resId, string authKey)
        {
            _resId = resId;
            _authKey = authKey;
        }

        public OneDriveFileDownloader(string resId, string authKey, string fileName) : this(resId, authKey)
        {
            FileName = fileName;
        }

        public async Task DownloadAsync()
        {
            Console.WriteLine($"Download file from OneDrive... [ResId: {_resId}, AuthKey: ***]");
            
            using (var client = new HttpClient())
            {
                var url = $"https://onedrive.live.com/download?resid={_resId}&authkey={_authKey}";
                
                using (var result = await client.GetStreamAsync(url))
                using(var file = new FileStream(FileName, FileMode.Create))
                {
                    await result.CopyToAsync(file);
                }
            }
        }
    }
}