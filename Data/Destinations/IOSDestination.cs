using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class IOSDestination : IDestination
    {
        private readonly IList<DestinationResourceWriterConfigSingleLanguage> _config;

        public IOSDestination(IList<DestinationResourceWriterConfigSingleLanguage> config)
        {
            _config = config;
        }

        public async Task WriteAsync(Document document)
        {
            foreach (var config in _config)
            {
                var writer = new IOSStringsResourceWriter(config);
                await writer.WriteAsync(document);
            }
        }
    }
}