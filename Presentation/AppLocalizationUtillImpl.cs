using System.Threading.Tasks;
using AppLocalizationUtil.Data.Configuration;
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

            document.ToString();
        }
    }
}