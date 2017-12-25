using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Configuration;
using AppLocalizationUtil.Data.Loaders;
using AppLocalizationUtil.Data.Sources;
using AppLocalizationUtil.Domain.Source;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            IConfigurationReader reader = new ConfigurationReader("Config.json");
            var configuration = await reader.ReadAsync();

            ISourceChooser sourceChooser = new SourceChooser();
            ISource source = sourceChooser.Choose(configuration.Source);
            
            var document = await source.LoadAsync();

            document.ToString();
        }
    }
}
