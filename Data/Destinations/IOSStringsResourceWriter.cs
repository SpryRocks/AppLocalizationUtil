using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class IOSStringsResourceWriter
    {
       private readonly DestinationResourceWriterConfigSingleLanguage _writerConfig;

        public IOSStringsResourceWriter(DestinationResourceWriterConfigSingleLanguage writerConfig)
        {
            _writerConfig = writerConfig;
        }

        public async Task WriteAsync(Document document)
        {
            Console.WriteLine($"Write iOS resource .strings file... [{_writerConfig.FileName}]");

            var language = document.Languages.Single(l => l.Id == _writerConfig.LanguageId);
            
            IDictionary<Group, IList<LocalizationItem>> filteredGroups = new Dictionary<Group, IList<LocalizationItem>>();

            foreach (var group in document.Groups) 
            {
                IList<LocalizationItem> items = new List<LocalizationItem>();

                foreach (var item in group.Items) 
                {
                    var isFilterCheckPassed = 
                        ApplyAppFilter(item, _writerConfig.AppsFilter) && 
                        ApplyIOSPlatformFilter(item) &&
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

            var fi = new FileInfo(_writerConfig.FileName);
            var di = fi.Directory;
            Debug.Assert(di != null, nameof(di) + " != null");
            if (!di.Exists)
                di.Create();

            using (var sr = new StreamWriter(_writerConfig.FileName, false, Encoding.UTF8))
            {             
                foreach (var filteredGroup in filteredGroups)
                {
                    var group = filteredGroup.Key;
                    var items = filteredGroup.Value;

                    await sr.WriteLineAsync($"// {group.Name}");

                    foreach (var item in items)
                    {
                        var key = item.Keys[Platforms.IOS];
                        var value = item.Values[language];

                        await sr.WriteLineAsync($"\"{key}\" = \"{PrepareValue(value)}\";");
                    }

                    await sr.WriteLineAsync();
                }

            }
        }

        private string PrepareValue(string value)
        {
            value = value.Trim();

            value = value
                .Replace("\"", "\\\"");

            value = DestinationValueFormatUtils.PrepareValueFormats(value, "s", "@", 1);
            
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

        private bool ApplyIOSPlatformFilter(LocalizationItem item)
        {
            return item.Keys.ContainsKey(Platforms.IOS);
        }

        private bool ApplyLanguageFilter(LocalizationItem item, Language language)
        {
            return item.Values.ContainsKey(language);
        }
    }
}