using System;
using System.Collections.Generic;
using System.Linq;
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
        static void Main(string[] args_)
        {
            try
            {
                var args = ParseArgs(args_);

                string configFileArg = "--ConfigFile";
                string configFileName = args.SingleOrDefault(arg => arg.key == configFileArg).value;
                if (string.IsNullOrEmpty(configFileName))
                    throw new Exception($"Please, specify '{configFileArg}' argument");

                AppLocalizationUtillImpl.Run(configFileName).Wait();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static (string key, string value)[] ParseArgs(string[] args)
        {
            int count = args.Length / 2;

            var res = new (string key, string value)[count];

            for (int i = 0; i < count; i++)
            {
                var arg = res[i] = (args[i * 2], args[i * 2 + 1]);
                if (!arg.key.StartsWith("--"))
                    throw new Exception("Param key should start from '--' prefix");
            }

            return res;
        }
    }
}
