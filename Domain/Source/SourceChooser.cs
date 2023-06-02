using System;
using System.Collections.Generic;
using System.Linq;
using AppLocalizationUtil.Data.Loaders;
using AppLocalizationUtil.Data.Sources;
using AppLocalizationUtil.Entities;
using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Domain.Source
{
    public class SourceChooser : ISourceChooser
    {
        public static ISourceChooser Create()
        {
            return new SourceChooser();
        }

        private SourceChooser()
        {
        }

        public ISource Choose(JObject sourceConfig)
        {
            IDictionary<string, Func<JObject, ISource>> types = new Dictionary<string, Func<JObject, ISource>>
            {
                {"Excel", ChooseExcel},
                {"Crowdin", ChooseCrowdin},
            };

            var type = sourceConfig.Value<string>("Type");
            if (!types.ContainsKey(type))
                throw new Exception($"Type '{type}' is not known");
            return types[type](sourceConfig);
        }

        private ISource ChooseExcel(JObject sourceConfig)
        {
            var excelConfiguration = sourceConfig["Config"].ToObject<ExcelConfiguration>();
            IFileDocumentReader fileDocumentReader = new ExcelFileDocumentReader(excelConfiguration);

            IDictionary<string, Func<JObject, IFileDownloader>> placementTypes =
                new Dictionary<string, Func<JObject, IFileDownloader>>
                {
                    {"OneDrive", ChooseOneDriveFileDownloader},
                    {"GoogleDrive", ChooseGoogleDriveFileDownloader}
                };

            JObject placementConfig = sourceConfig.Value<JObject>("Placement");
            var placementType = placementConfig.Value<string>("Type");
            if (!placementTypes.ContainsKey(placementType))
                throw new Exception($"Placement type '{placementType}' is not known");
            var fileDownloader = placementTypes[placementType](placementConfig);

            return new RemoteFileSource(fileDownloader, fileDocumentReader);
        }

        private ISource ChooseCrowdin(JObject sourceConfig)
        {
            var crowdinPlacement = sourceConfig.Value<JObject>("Placement");
            var crowdinConfig = sourceConfig.Value<JObject>("Config");

            var apiKey = crowdinPlacement.Value<string>("ApiKey");
            var projectId = crowdinPlacement.Value<int>("ProjectId");
            var directory = crowdinPlacement.Value<string>("Directory");
            var defaultLanguageId = crowdinConfig.Value<string>("DefaultLanguageId");
            var defaultPlatform = crowdinConfig.Value<string>("DefaultPlatform");
            var languages = crowdinConfig.Value<JArray>("Languages");

            return new CrowdinSource(new Config
            {
                ApiKey = apiKey,
                ProjectId = projectId,
                Directory = directory,
                DefaultLanguageId = defaultLanguageId,
                DefaultPlatform = defaultPlatform,
                Languages = languages.Select(l => new Config.Language
                {
                    Id = l.Value<string>("Id"),
                    Name = l.Value<string>("Name"),
                    CrowdinId = l.Value<string>("CrowdinId"),
                }).ToArray(),
            });
        }

        private IFileDownloader ChooseOneDriveFileDownloader(JObject placementConfig)
        {
            var config = placementConfig["Config"];

            var resId = config.Value<string>("ResId");
            var authKey = config.Value<string>("AuthKey");

            string tmpFileName = $"{Environment.CurrentDirectory}/wrk.xlsx";

            return new OneDriveFileDownloader(resId, authKey, tmpFileName);
        }

        private IFileDownloader ChooseGoogleDriveFileDownloader(JObject placementConfig)
        {
            var config = placementConfig["Config"];

            var id = config.Value<string>("Id");

            string tmpFileName = $"{Environment.CurrentDirectory}/wrk.xlsx";

            return new GoogleDriveFileDownloader(id, tmpFileName);
        }
    }
}