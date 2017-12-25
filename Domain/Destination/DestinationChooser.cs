using System;
using System.Collections.Generic;
using System.Linq;
using AppLocalizationUtil.Data.Destinations;
using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Domain.Destination
{
    public class DestinationChooser : IDestinationChooser
    {
        private readonly string _destinationPath;

        public DestinationChooser(string destinationPath)
        {
            _destinationPath = destinationPath;
        }

        public IList<IDestination> Choose(IList<JObject> destinationConfigs)
        {
            return destinationConfigs.Select(Choose).ToList();
        }

        private IDestination Choose(JObject destinationConfig)
        {
            IDictionary<string, Func<JObject, IDestination>> destinations = new Dictionary<string, Func<JObject, IDestination>>
            {
                { Platforms.Android, ChooseAndroid },
                { Platforms.Web, ChooseWeb }
            };

            string type = destinationConfig.Value<string>("Type");
            if (!destinations.ContainsKey(type))
                throw new Exception($"Type '{type}' is not known");
            return destinations[type](destinationConfig);
        }

        private IDestination ChooseAndroid(JObject destinationConfig)
        {
            string path = _destinationPath;
            
            if (path == null)
                path = string.Empty;

            string destinationPath = destinationConfig.Value<string>("Path");
            if (destinationPath != null)
            {
                if (!string.IsNullOrEmpty(path))
                    path += "/";
                path += destinationPath;
            }

            IList<AndroidXmlResourceWriter> writers = new List<AndroidXmlResourceWriter>();

            IDictionary<string, string> languageId_files = destinationConfig["Files"].ToObject<IDictionary<string, string>>();
            IDictionary<string, string> filter = destinationConfig["Filter"].ToObject<IDictionary<string, string>>();

            string appFilter = filter["App"];
            
            foreach (var languageId_file in languageId_files)
            {
                string languageId = languageId_file.Key;
                string fileConfig = languageId_file.Value;

                string filePath = path;
                if (!string.IsNullOrEmpty(filePath))
                    filePath += "/";
                filePath += fileConfig;

                writers.Add(new AndroidXmlResourceWriter(filePath, languageId, appFilter));
            }

            return new AndroidDestination(writers);
        }

        private IDestination ChooseWeb(JObject destinationConfig)
        {
            return new WebDestination();
        }
    }
}