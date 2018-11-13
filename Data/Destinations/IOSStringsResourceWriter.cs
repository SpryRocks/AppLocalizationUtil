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
        private readonly string _fileName;
        private readonly string _languageId;
        private readonly IList<string> _appsFilter;

        public IOSStringsResourceWriter(string fileName, string languageId, IList<string> appsFilter)
        {
            _fileName = fileName;
            _languageId = languageId;
            _appsFilter = appsFilter;
        }

        public async Task WriteAsync(Document document)
        {
            Console.WriteLine($"Write iOS resource .strings file... [{_fileName}]");

            var language = document.Languages.Single(l => l.Id == _languageId);
            
            IDictionary<Group, IList<LocalizationItem>> filteredGroups = new Dictionary<Group, IList<LocalizationItem>>();

            foreach (var group in document.Groups) 
            {
                IList<LocalizationItem> items = new List<LocalizationItem>();

                foreach (var item in group.Items) 
                {
                    var isFilterCheckPassed = 
                        ApplyAppFilter(item, _appsFilter) && 
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

            var fi = new FileInfo(_fileName);
            var di = fi.Directory;
            Debug.Assert(di != null, nameof(di) + " != null");
            if (!di.Exists)
                di.Create();

            using (var sr = new StreamWriter(_fileName, false, Encoding.UTF8))
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
                .Replace("\"", "\\\"")
                .Replace("%s", "%@");

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