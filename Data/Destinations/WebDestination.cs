using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class WebDestination : IDestination
    {
        public async Task WriteAsync(Document document)
        {
            await Task.Run(() => {});
        }
    }
}