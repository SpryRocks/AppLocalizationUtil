using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class WebDestination : IDestination
    {
        private readonly IList<DestinationResourceWriterConfigBase> _config;

        public WebDestination(IList<DestinationResourceWriterConfigBase> config)
        {
            _config = config;
        }

        public async Task WriteAsync(Document document)
        {
            foreach (var config in _config)
            {
                var writer = new WebJsonResourceWriter(config);
                await writer.WriteAsync(document);
            }
        }
    }
}
