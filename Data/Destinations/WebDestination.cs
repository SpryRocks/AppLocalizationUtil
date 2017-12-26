using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class WebDestination : IDestination
    {
        private readonly WebJsonResourceWriter _writer;

        public WebDestination(WebJsonResourceWriter writer)
        {
            _writer = writer;
        }

        public async Task WriteAsync(Document document)
        {
            await _writer.WriteAsync(document);
        }
    }
}