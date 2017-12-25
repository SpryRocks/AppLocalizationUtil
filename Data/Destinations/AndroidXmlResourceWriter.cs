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
        private readonly Language _language;
        private readonly string _appFilter;

        public AndroidXmlResourceWriter(string fileName, Language language, string appFilter)
        {
            _fileName = fileName;
            _language = language;
            _appFilter = appFilter;
        }

        public async Task WriteAsync(Document document)
        {
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
                        ApplyAppFilter(item) && 
                        ApplyPlatformFilter(item) &&
                        ApplyLanguageFilter(item);
                    
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
                    var value = item.Values[_language];
                    
                    var xStringt = new XElement("string", new XAttribute("name", key));
                    xStringt.Value = PrepareValue(value);
                    xResources.Add(xStringt);
                }
            }

            using(var file = new FileStream(_fileName, FileMode.Create))
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

            return value;
        }

        private bool ApplyAppFilter(LocalizationItem item) 
        {
            if (_appFilter == null)
            {
                return item.Apps.Count < 1;
            }
            else
            {
                return item.Apps.Contains(_appFilter);
            }
        }

        private bool ApplyPlatformFilter(LocalizationItem item)
        {
            return item.Keys.ContainsKey(Platforms.Android);
        }

        private bool ApplyLanguageFilter(LocalizationItem item)
        {
            return item.Values.ContainsKey(_language);
        }
    }
}