using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class IOSDestination : IDestination
    {
        private readonly IList<IOSStringsResourceWriter> _writers;

        public IOSDestination(IList<IOSStringsResourceWriter> writers)
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