using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Configuration;
using AppLocalizationUtil.Data.Loaders;
using AppLocalizationUtil.Data.Sources;
using AppLocalizationUtil.Domain.Source;
using AppLocalizationUtil.Entities;
using AppLocalizationUtil.Presentation;

namespace AppLocalizationUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFileName = "Config.json";
            
            AppLocalizationUtillImpl.Run(configFileName).Wait();
        }   
    }
}
