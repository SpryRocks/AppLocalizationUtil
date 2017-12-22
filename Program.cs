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
                var resId = "C62576FDA2F2250C!1418";
                var authKey = "!AOiLnVvvWMZtbv4";
                var url = $"https://onedrive.live.com/download?resid={resId}&authkey={authKey}";
                
                using (var result = await client.GetStreamAsync(url))
                {
                    using(var file = new FileStream(Environment.CurrentDirectory + "/wrk.xlsx", FileMode.Create)) {
                        await result.CopyToAsync(file);
                    }
                }
            }
        }
    }
}
