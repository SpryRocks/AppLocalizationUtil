using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Configuration;
using AppLocalizationUtil.Data.Destinations;
using AppLocalizationUtil.Data.Sources;
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

            IList<IDestination> destinations = new List<IDestination>();

            foreach (var destination in destinations)
            {
                await destination.WriteAsync(document);
            }
    
            var writer = new AndroidXmlResourceWriter(
                fileName: "C:/Users/maxim/Documents/git/saqure-app-android/Sources/SaQure/saqure.app.presentation/src/main/res/values/strings.xml",
                language: document.Languages.Where(l => l.Id == "en").Single(),
                appFilter: null
            );

            await writer.WriteAsync(document);
        }
    }
}