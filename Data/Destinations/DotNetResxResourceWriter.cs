using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources.NetStandard;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class DotNetResxResourceWriter
    {
        private readonly DestinationResourceWriterConfigSingleLanguage _writerConfig;

        public DotNetResxResourceWriter(DestinationResourceWriterConfigSingleLanguage writerConfig)
        {
            _writerConfig = writerConfig;
        }

        public Task WriteAsync(Document document)
        {
            return Task.Run(() => WriteInternal(document));
        }
        
        private void WriteInternal(Document document)
        {
            Console.WriteLine($"Write .Net resource .resx file... [{_writerConfig.FileName}]");

            var language = document.FindLanguage(_writerConfig.LanguageId);

            using (var writer = new ResXResourceWriter(_writerConfig.FileName)
            {
                CustomReaderValue =
                    "System.Resources.ResXResourceReader, System.Windows.Forms, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                CustomWriterValue =
                    "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            })
            {

                IDictionary<Group, IList<LocalizationItem>> filteredGroups =
                    new Dictionary<Group, IList<LocalizationItem>>();

                foreach (var group in document.Groups)
                {
                    IList<LocalizationItem> items = new List<LocalizationItem>();

                    foreach (var item in group.Items)
                    {
                        var isFilterCheckPassed =
                            ApplyAppFilter(item, _writerConfig.AppsFilter) &&
                            ApplyDotNetPlatformFilter(item) &&
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

                    //xResources.Add(new XComment($" {group.Name} "));

                    foreach (var item in items)
                    {
                        var key = item.Keys[Platforms.DotNet];
                        var value = item.Values[language];

                        writer.AddResource(key, PrepareValue(value));
                    }
                }

                var fi = new FileInfo(_writerConfig.FileName);
                var di = fi.Directory;
                Debug.Assert(di != null, nameof(di) + " != null");
                if (!di.Exists)
                    di.Create();
            }

            DeleteDesignerFile();
        }

        private void DeleteDesignerFile()
        {
            var resxInfo = new FileInfo(_writerConfig.FileName);
            var resxName = resxInfo.Name;

            var designerFileName = resxInfo.DirectoryName! + "\\" +
                                   resxName[..^resxInfo.Extension.Length] + ".Designer.cs";

            if (!File.Exists(designerFileName)) return;
            
            Console.WriteLine($"Delete .Net resource designer file... [{designerFileName}]");
            File.Delete(designerFileName);
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
        
        private bool ApplyDotNetPlatformFilter(LocalizationItem item)
        {
            return item.Keys.ContainsKey(Platforms.DotNet);
        }

        private bool ApplyLanguageFilter(LocalizationItem item, Language language)
        {
            return item.Values.ContainsKey(language);
        }
        
        private string PrepareValue(string value)
        {
            value = value.Trim();
            
            // if (value.StartsWith("?"))
            // {
            //     value = "\\" + value;
            // }
            //
            // value = value
            //     .Replace("'", "\\\'")
            //     .Replace("\n", "\\n")
            //     .Replace("...", "…");
            //
            // value = DestinationValueFormatUtils.PrepareValueFormats(value, "s", "s", 1);

            return value;
        }
    }
}