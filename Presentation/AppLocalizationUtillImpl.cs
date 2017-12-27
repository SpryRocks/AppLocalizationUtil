using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Configuration;
using AppLocalizationUtil.Data.Destinations;
using AppLocalizationUtil.Data.Sources;
using AppLocalizationUtil.Domain.Destination;
using AppLocalizationUtil.Domain.Source;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Presentation
{
    public class AppLocalizationUtillImpl
    {
        public static async Task Run(string configFileName)
        {
            Console.WriteLine("--- App localization util ---");

            Console.WriteLine($"Current directory: {Environment.CurrentDirectory}");
            
            IConfigurationReader configurationReader = new ConfigurationReader(configFileName);
            Console.WriteLine("[Configuration]");
            var configuration = await configurationReader.ReadAsync();

            Document document;

            ISourceChooser sourceChooser = new SourceChooser();
            Console.WriteLine("[Source]");
            using (ISource source = sourceChooser.Choose(configuration.Source))
            {
                document = await source.LoadAsync();
            }

            IDestinationChooser destinationChooser = new DestinationChooser(configuration.DestinationPath);
            IList<IDestination> destinations = destinationChooser.Choose(configuration.Destinations);

            Console.WriteLine("[Destination]");
            foreach (var destination in destinations)
            {
                await destination.WriteAsync(document);
            }
        }
    }
}