using System;
using System.Collections.Generic;
using System.Linq;
using AppLocalizationUtil.Data.Destinations;
using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Domain.Destination
{
    public class DestinationChooser : IDestinationChooser
    {
        public static IDestinationChooser Create(string destinationPath)
        {
            return new DestinationChooser(destinationPath);
        }

        private readonly string _destinationPath;

        private DestinationChooser(string destinationPath)
        {
            _destinationPath = destinationPath;
        }

        public IList<IDestination> Choose(IList<JObject> destinationConfigs)
        {
            return destinationConfigs.Select(Choose).ToList();
        }

        private IDestination Choose(JObject destinationConfig)
        {
            IDictionary<string, Func<JObject, IDestination>> destinations =
                new Dictionary<string, Func<JObject, IDestination>>
                {
                    {Platforms.Android, ChooseAndroid},
                    {Platforms.Web, ChooseWeb},
                    {Platforms.IOS, ChooseIOS},
                    {Platforms.DotNet, ChooseDotNet},
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

            IList<DestinationResourceWriterConfigSingleLanguage> config =
                new List<DestinationResourceWriterConfigSingleLanguage>();

            IDictionary<string, string> languageId_files =
                destinationConfig["Files"].ToObject<IDictionary<string, string>>();
            IDictionary<string, JContainer> filter =
                destinationConfig["Filter"].ToObject<IDictionary<string, JContainer>>();

            IList<string> appsFilter = filter["App"].ToObject<IList<string>>();

            foreach (var languageId_file in languageId_files)
            {
                string languageId = languageId_file.Key;
                string fileConfig = languageId_file.Value;

                string filePath = path;
                if (!string.IsNullOrEmpty(filePath))
                    filePath += "/";
                filePath += fileConfig;

                config.Add(new DestinationResourceWriterConfigSingleLanguage(filePath, languageId, appsFilter));
            }

            return new AndroidDestination(config);
        }

        private IDestination ChooseWeb(JObject destinationConfig)
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

            IDictionary<string, JContainer> filter =
                destinationConfig["Filter"].ToObject<IDictionary<string, JContainer>>();
            IList<string> appsFilter = filter["App"].ToObject<IList<string>>();

            IDictionary<string, string> languageId_files =
                destinationConfig["Files"]?.ToObject<IDictionary<string, string>>();
            var omitLanguageObject = destinationConfig.Value<bool>("OmitLanguageObject");
            if (languageId_files != null)
            {
                var config = new List<DestinationResourceWriterConfigBase>();
               
                foreach (var languageId_file in languageId_files)
                {
                    string languageId = languageId_file.Key;
                    string fileConfig = languageId_file.Value;

                    string filePath = path;
                    if (!string.IsNullOrEmpty(filePath))
                        filePath += "/";
                    filePath += fileConfig;

                    config.Add(new DestinationResourceWriterConfigSingleLanguage(filePath, languageId, appsFilter, omitLanguageObject));
                }
                
                return new WebDestination(config);
            }
            else
            {
                string fileConfig = destinationConfig.Value<string>("File");
                
                string filePath = path;
                if (!string.IsNullOrEmpty(filePath))
                    filePath += "/";
                filePath += fileConfig;

                var config = new List<DestinationResourceWriterConfigBase>
                {
                    new DestinationResourceWriterConfigMultiLanguage(filePath, languageIdsFilter: null,
                        appsFilter: appsFilter, omitLanguageObject)
                };
                return new WebDestination(config);
            }
        }

        private IDestination ChooseIOS(JObject destinationConfig)
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

            IList<DestinationResourceWriterConfigSingleLanguage> config =
                new List<DestinationResourceWriterConfigSingleLanguage>();

            IDictionary<string, string> languageId_files =
                destinationConfig["Files"].ToObject<IDictionary<string, string>>();
            IDictionary<string, JContainer> filter =
                destinationConfig["Filter"].ToObject<IDictionary<string, JContainer>>();

            IList<string> appsFilter = filter["App"].ToObject<IList<string>>();

            foreach (var languageId_file in languageId_files)
            {
                string languageId = languageId_file.Key;
                string fileConfig = languageId_file.Value;

                string filePath = path;
                if (!string.IsNullOrEmpty(filePath))
                    filePath += "/";
                filePath += fileConfig;

                config.Add(new DestinationResourceWriterConfigSingleLanguage(filePath, languageId, appsFilter));
            }

            return new IOSDestination(config);
        }

        private IDestination ChooseDotNet(JObject destinationConfig)
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

            IList<DestinationResourceWriterConfigSingleLanguage> writers =
                new List<DestinationResourceWriterConfigSingleLanguage>();

            IDictionary<string, string> languageId_files =
                destinationConfig["Files"].ToObject<IDictionary<string, string>>();
            IDictionary<string, JContainer> filter =
                destinationConfig["Filter"].ToObject<IDictionary<string, JContainer>>();

            IList<string> appsFilter = filter["App"].ToObject<IList<string>>();

            foreach (var languageId_file in languageId_files)
            {
                string languageId = languageId_file.Key;
                string fileConfig = languageId_file.Value;

                string filePath = path;
                if (!string.IsNullOrEmpty(filePath))
                    filePath += "/";
                filePath += fileConfig;

                writers.Add(new DestinationResourceWriterConfigSingleLanguage(filePath, languageId, appsFilter));
            }

            return new DotNetDestination(writers);
        }
    }
}