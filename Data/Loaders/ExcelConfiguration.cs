using System.Collections.Generic;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Loaders
{
    public class ExcelConfiguration
    {
        public IDictionary<string, Language> LanguageColumns { get; set; }
        public IDictionary<string, string> PlatformKeyColumns { get; set; }
        public string AppsColumn { get; set; }
    }
}