using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class DotNetDestination:IDestination
    {
        private readonly IList<DestinationResourceWriterConfigSingleLanguage> _config;

        public DotNetDestination(IList<DestinationResourceWriterConfigSingleLanguage> config)
        {
            _config = config;
        }

        public async Task WriteAsync(Document document)
        {
            foreach (var config in _config)
            {
                var writer = new DotNetResxResourceWriter(config);
                await writer.WriteAsync(document);
            }
        }
    }
}