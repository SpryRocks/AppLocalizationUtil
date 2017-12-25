using System;
using System.Collections.Generic;
using AppLocalizationUtil.Data.Loaders;
using AppLocalizationUtil.Data.Sources;
using AppLocalizationUtil.Entities;
using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Domain.Source
{
    public class SourceChooser : ISourceChooser
    {
        public ISource Choose(JObject sourceConfig)
        {
            IDictionary<string, Func<JObject, ISource>> types = new Dictionary<string, Func<JObject, ISource>>
            {
                { "Excel", ChooseExcel }
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

            IDictionary<string, Func<JObject, IFileDownloader>> placementTypes = new Dictionary<string, Func<JObject, IFileDownloader>>
            {
                { "OneDrive", ChooseOneDriveFileDownloader }
            };

            JObject placementConfig = sourceConfig.Value<JObject>("Placement");
            var placementType = placementConfig.Value<string>("Type");
            if (!placementTypes.ContainsKey(placementType))
                throw new Exception($"Placement type '{placementType}' is not known");
            var fileDownloader = placementTypes[placementType](placementConfig);
            
            return new RemoteFileSource(fileDownloader, fileDocumentReader);
        }

        private IFileDownloader ChooseOneDriveFileDownloader(JObject placementConfig)
        {
            var config = placementConfig["Config"];

            var resId = config.Value<string>("ResId");
            var authKey = config.Value<string>("AuthKey");

            string tmpFileName = $"{Environment.CurrentDirectory}/wrk.xlsx";

            return new OneDriveFileDownloader(resId, authKey, tmpFileName);
        }
    }
}