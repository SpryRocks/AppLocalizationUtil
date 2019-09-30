using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Data.Destinations
{
    public class WebJsonResourceWriter
    {
        private readonly string _fileName;
        private readonly IList<string> _languageIdsFilter;
        private readonly IList<string> _appsFilter;

        public WebJsonResourceWriter(string fileName, IList<string> languageIdsFilter, IList<string> appsFilter)
        {
            _fileName = fileName;
            _languageIdsFilter = languageIdsFilter;
            _appsFilter = appsFilter;
        }

        public async Task WriteAsync(Document document)
        {
            Console.WriteLine($"Write Web resource .json file... [{_fileName}]");

            IDictionary<Language, LanguageGroup> languages = new Dictionary<Language, LanguageGroup>();

            foreach (var language in document.Languages) 
            {
                if (!ApplyLanguageFilter(language))
                    continue;

                languages.Add(language, new LanguageGroup());
            }

            foreach (var group in document.Groups)
            {
                foreach (var item in group.Items)
                {
                    if (!ApplyWebPlatformFilter(item))
                        continue;
                    if (!ApplyAppFilter(item, _appsFilter))
                        continue;

                    var keys = SplitKey(item.Keys[Platforms.Web]);

                    foreach (var value in item.Values)
                    {
                        var language = value.Key;

                        if (!languages.ContainsKey(language))
                            continue;

                        var languageGroup = languages[language];

                        foreach (var key in keys)
                        {
                            NamespaceGroup namespaceGroup;
                            if (!languageGroup.Namespaces.ContainsKey(key.ns))
                            {
                                namespaceGroup = new NamespaceGroup();
                                languageGroup.Namespaces.Add(key.ns, namespaceGroup);
                            }
                            else
                            {
                                namespaceGroup = languageGroup.Namespaces[key.ns];
                            }

                            namespaceGroup.Translations.Add(key.value, value.Value);
                        }
                    }
                }
            }

            var jDocument = new JObject();

            foreach (var language in languages)
            {
                var languageId = language.Key.Id;

                var jLanguage =  new JObject();

                jDocument.Add(new JProperty(languageId, jLanguage));

                foreach (var ns in language.Value.Namespaces)
                {
                    var jNs = new JObject();

                    jLanguage.Add(ns.Key, jNs);

                    foreach (var item in ns.Value.Translations)
                    {
                        jNs.Add(new JProperty(item.Key, item.Value));
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(_fileName))
            using (JsonWriter writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented })
            {
                await jDocument.WriteToAsync(writer);
                await sw.WriteLineAsync();
            }
        }

        private bool ApplyLanguageFilter(Language language)
        {
            return _languageIdsFilter == null || _languageIdsFilter.Contains(language.Id);
        }

        private bool ApplyWebPlatformFilter(LocalizationItem item)
        {
            return item.Keys.ContainsKey(Platforms.Web);
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

        private (string ns, string value)[] SplitKey(string key)
        {
            return key.Split(';').Select(s =>
            {
                string[] parts = s.Split(':');
                if (parts.Length != 2)
                    throw new Exception($"Bad key format: '{s}'. Key should be separated using symbol ':'");
                
                return (parts[0], parts[1]);
            }).ToArray();
        }

        private class LanguageGroup
        {
            public IDictionary<string, NamespaceGroup> Namespaces { get; } = new Dictionary<string, NamespaceGroup>();
        }

        private class NamespaceGroup
        {
            public IDictionary<string, string> Translations { get; } = new Dictionary<string, string>();
        }
    }
}