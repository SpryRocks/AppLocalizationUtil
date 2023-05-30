using System;
using System.Collections.Generic;
using System.Linq;

namespace AppLocalizationUtil.Entities
{
    public class Document
    {
        public IList<Group> Groups { get; set; }
        public ISet<Language> Languages { get; set; }
        public ISet<string> Platforms { get; set; }

        public Language FindLanguage(string languageId)
        {
            var found = Languages.SingleOrDefault(l => l.Id == languageId);
            if (found == null) throw new Exception($"Language not found in the document: {languageId}");
            return found;
        }
    }
}
