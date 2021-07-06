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
            
            var configurationReader = ConfigurationReader.Create(configFileName);
            Console.WriteLine("[Configuration]");
            var configuration = await configurationReader.ReadAsync();

            Document document;

            var sourceChooser = SourceChooser.Create();
            Console.WriteLine("[Source]");
            using (var source = sourceChooser.Choose(configuration.Source))
            {
                document = await source.LoadAsync();
            }

            var destinationChooser = DestinationChooser.Create(configuration.DestinationPath);
            var destinations = destinationChooser.Choose(configuration.Destinations);

            Console.WriteLine("[Destination]");
            foreach (var destination in destinations)
            {
                await destination.WriteAsync(document);
            }
        }
    }
}