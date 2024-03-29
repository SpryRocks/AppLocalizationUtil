﻿using System.Collections.Generic;

namespace AppLocalizationUtil.Data.Destinations
{
    public class DestinationResourceWriterConfigBase
    {
        public string FileName { get; set; }
        public IList<string> AppsFilter { get; set; }
        public bool OmitLanguageObject { get; set; }

        public DestinationResourceWriterConfigBase(
            string fileName,
            IList<string> appsFilter,
            bool omitLanguageObject
        )
        {
            FileName = fileName;
            AppsFilter = appsFilter;
            OmitLanguageObject = omitLanguageObject;
        }
    }

    public class DestinationResourceWriterConfigSingleLanguage : DestinationResourceWriterConfigBase
    {
        public string LanguageId { get; set; }

        public DestinationResourceWriterConfigSingleLanguage(
            string fileName,
            string languageId,
            IList<string> appsFilter,
            bool omitLanguageObject = false
        ) : base(fileName, appsFilter, omitLanguageObject)
        {
            LanguageId = languageId;
        }
    }

    public class DestinationResourceWriterConfigMultiLanguage : DestinationResourceWriterConfigBase
    {
        public IList<string> LanguageIdsFilter { get; set; }

        public DestinationResourceWriterConfigMultiLanguage(
            string fileName,
            IList<string> languageIdsFilter,
            IList<string> appsFilter,
            bool omitLanguageObject = false
        ) : base(fileName, appsFilter, omitLanguageObject)
        {
            LanguageIdsFilter = languageIdsFilter;
        }
    }
}