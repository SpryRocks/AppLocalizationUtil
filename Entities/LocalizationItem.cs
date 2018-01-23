using System.Collections.Generic;
using System.Linq;

namespace AppLocalizationUtil.Entities
{
    public class LocalizationItem
    {
        public IDictionary<string, string> Keys { get; set; }
        public IDictionary<Language, string> Values { get; set; }
        public ISet<string> Apps { get; set; }

        public override string ToString()
        {
            return string.Join(", ", Values.Select(it => $"{it.Key.Id}:{it.Value}"));
        }
    }
}