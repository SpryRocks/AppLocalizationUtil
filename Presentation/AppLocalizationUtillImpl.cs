using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Configuration;
using AppLocalizationUtil.Data.Destinations;
using AppLocalizationUtil.Data.Sources;
using AppLocalizationUtil.Domain.Destination;
using AppLocalizationUtil.Domain.Source;

namespace AppLocalizationUtil.Presentation
{
    public class AppLocalizationUtillImpl
    {
        public static async Task Run(string configFileName)
        {
            IConfigurationReader configurationReader = new ConfigurationReader(configFileName);
            var configuration = await configurationReader.ReadAsync();

            ISourceChooser sourceChooser = new SourceChooser();
            ISource source = sourceChooser.Choose(configuration.Source);
            
            var document = await source.LoadAsync();

            IDestinationChooser destinationChooser = new DestinationChooser(configuration.DestinationPath);
            IList<IDestination> destinations = destinationChooser.Choose(configuration.Destinations);

            foreach (var destination in destinations)
            {
                await destination.WriteAsync(document);
            }
        }
    }
}