using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

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
            using (var client = new HttpClient())
            {
                using (var result = await client.GetStreamAsync("https://onedrive.live.com/download?resid=C62576FDA2F2250C!1418&ithint=file%2cxlsx&app=Excel&wdo=2&authkey=!AHpmF6oJPcGoeSw"))
                {
                    using(var file = new FileStream(Environment.CurrentDirectory + "/wrk.xlsx", FileMode.Create)) {
                        await result.CopyToAsync(file);
                    }
                }
            }
        }
    }
}
