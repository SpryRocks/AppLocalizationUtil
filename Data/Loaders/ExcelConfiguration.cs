using System.Collections.Generic;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Loaders
{
    public class ExcelConfiguration
    {
        public IDictionary<string, Language> LanguageColumns { get; set; }
    }
}