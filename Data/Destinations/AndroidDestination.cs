using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class AndroidDestination : IDestination
    {
        private readonly IList<AndroidXmlResourceWriter> _writers;

        public AndroidDestination(IList<AndroidXmlResourceWriter> writers)
        {
            _writers = writers;
        }

        public async Task WriteAsync(Document document)
        {
            foreach (var writer in _writers)
            {
                await writer.WriteAsync(document);
            }
        }
    }
}