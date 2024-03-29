using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class AndroidDestination : IDestination
    {
        private readonly IList<DestinationResourceWriterConfigSingleLanguage> _config;

        public AndroidDestination(IList<DestinationResourceWriterConfigSingleLanguage> config)
        {
            _config = config;
        }

        public async Task WriteAsync(Document document)
        {
            foreach (var config in _config)
            {
                var writer = new AndroidXmlResourceWriter(config);
                await writer.WriteAsync(document);
            }
        }
    }
}