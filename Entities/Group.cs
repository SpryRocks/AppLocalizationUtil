using System.Collections.Generic;

namespace AppLocalizationUtil.Entities
{
    public class Group
    {
        public string Name { get; set; }
        public IList<LocalizationItem> Items { get; set; }
    }
}