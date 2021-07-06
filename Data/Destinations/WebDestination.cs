using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class WebDestination : IDestination
    {
        private readonly DestinationResourceWriterConfigMultiLanguage _config;

        public WebDestination(DestinationResourceWriterConfigMultiLanguage config)
        {
            _config = config;
        }

        public async Task WriteAsync(Document document)
        {
            var writer = new WebJsonResourceWriter(_config);
            await writer.WriteAsync(document);
        }
    }
}