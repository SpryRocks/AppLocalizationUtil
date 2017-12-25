using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class AndroidXmlResourceWriter
    {
        private readonly string _fileName;
        private readonly string _languageId;
        private readonly string _appFilter;

        public AndroidXmlResourceWriter(string fileName, string languageId, string appFilter)
        {
            _fileName = fileName;
            _languageId = languageId;
            _appFilter = appFilter;
        }

        public async Task WriteAsync(Document document)
        {
            var language = document.Languages.Single(l => l.Id == _languageId);
            
            var xDocument = new XDocument();
            var xResources = new XElement("resources");

            xDocument.Add(xResources);

            IDictionary<Group, IList<LocalizationItem>> filteredGroups = new Dictionary<Group, IList<LocalizationItem>>();

            foreach (var group in document.Groups) 
            {
                IList<LocalizationItem> items = new List<LocalizationItem>();

                foreach (var item in group.Items) 
                {
                    bool isFilterCheckPassed = 
                        ApplyAppFilter(item, _appFilter) && 
                        ApplyAndroidPlatformFilter(item) &&
                        ApplyLanguageFilter(item, language);
                    
                    if (isFilterCheckPassed)
                    {
                        items.Add(item);
                    }
                }

                if (items.Count > 0)
                {
                    filteredGroups.Add(group, items);
                }
            }

            foreach (var filteredGroup in filteredGroups)
            {
                var group = filteredGroup.Key;
                var items = filteredGroup.Value;
                
                xResources.Add(new XComment($" {group.Name} "));

                foreach (var item in items)
                {
                    var key = item.Keys[Platforms.Android];
                    var value = item.Values[language];
                    
                    var xStringt = new XElement("string", new XAttribute("name", key));
                    xStringt.Value = PrepareValue(value);
                    xResources.Add(xStringt);
                }
            }

            FileInfo fi = new FileInfo(_fileName);
            DirectoryInfo di = fi.Directory;
            if (!di.Exists)
                di.Create();

            using(var file = new FileStream(fi.FullName, FileMode.Create))
            {
                await xDocument.SaveAsync(file, SaveOptions.None, CancellationToken.None);
            }
        }

        private string PrepareValue(string value)
        {
            value = value.Trim();
            
            if (value.StartsWith("?"))
            {
                value = "/" + value;
            }

            value = value.Replace("'", "\'");

            return value;
        }

        private bool ApplyAppFilter(LocalizationItem item, string appFilter) 
        {
            if (appFilter == null)
            {
                return item.Apps.Count < 1;
            }
            else
            {
                return item.Apps.Contains(appFilter);
            }
        }

        private bool ApplyAndroidPlatformFilter(LocalizationItem item)
        {
            return item.Keys.ContainsKey(Platforms.Android);
        }

        private bool ApplyLanguageFilter(LocalizationItem item, Language language)
        {
            return item.Values.ContainsKey(language);
        }
    }
}