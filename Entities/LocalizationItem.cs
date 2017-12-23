using System.Collections.Generic;

namespace AppLocalizationUtil.Entities
{
    public class LocalizationItem
    {
        public IDictionary<string, string> Keys { get; set; }
        public IDictionary<Language, string> Values { get; set; }
        public ISet<string> Apps { get; set; }
    }
}