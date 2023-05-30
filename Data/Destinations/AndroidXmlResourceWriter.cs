using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class AndroidXmlResourceWriter
    {
        private readonly DestinationResourceWriterConfigSingleLanguage _writerConfig;

        public AndroidXmlResourceWriter(DestinationResourceWriterConfigSingleLanguage writerConfig)
        {
            _writerConfig = writerConfig;
        }

        public async Task WriteAsync(Document document)
        {
            Console.WriteLine($"Write Android resource .xml file... [{_writerConfig.FileName}]");

            var language = document.FindLanguage(_writerConfig.LanguageId);
            
            var xDocument = new XDocument();
            var xResources = new XElement("resources");

            xDocument.Add(xResources);

            IDictionary<Group, IList<LocalizationItem>> filteredGroups = new Dictionary<Group, IList<LocalizationItem>>();

            foreach (var group in document.Groups) 
            {
                IList<LocalizationItem> items = new List<LocalizationItem>();

                foreach (var item in group.Items) 
                {
                    var isFilterCheckPassed = 
                        ApplyAppFilter(item, _writerConfig.AppsFilter) && 
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

                    var xStringt = new XElement("string", new XAttribute("name", key)) {Value = PrepareValue(value)};
                    xResources.Add(xStringt);
                }
            }

            var fi = new FileInfo(_writerConfig.FileName);
            var di = fi.Directory;
            Debug.Assert(di != null, nameof(di) + " != null");
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
                value = "\\" + value;
            }

            value = value
                .Replace("'", "\\\'")
                .Replace("\n", "\\n")
                .Replace("...", "…");
          
            value = DestinationValueFormatUtils.PrepareValueFormats(value, "s", "s", 1);

            return value;
        }

        private bool ApplyAppFilter(LocalizationItem item, IList<string> appsFilter) 
        {
            foreach (var appFilter in appsFilter)
            {
                if (appFilter == null)
                {
                    if (item.Apps.Count < 1)
                        return true;
                }
                else
                {
                    if (item.Apps.Contains(appFilter))
                        return true;
                }
            }

            return false;
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