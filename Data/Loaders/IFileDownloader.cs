using System.IO;
using System.Threading.Tasks;

namespace AppLocalizationUtil.Data.Loaders
{
    public interface IFileDownloader
    {
        string FileName { get; set; }
        
        Task DownloadAsync();
    }
}