using System.Collections.Generic;

namespace AppLocalizationUtil.Entities
{
    public class Document
    {
        public IList<Group> Groups { get; set; }
        public ISet<Language> Languages { get; set; }
        public ISet<string> Platforms { get; set; }
    }
}