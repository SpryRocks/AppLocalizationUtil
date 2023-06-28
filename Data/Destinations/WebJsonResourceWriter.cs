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
        private readonly DestinationResourceWriterConfigBase _writerConfig;

        public WebJsonResourceWriter(DestinationResourceWriterConfigBase writerConfig)
        {
            _writerConfig = writerConfig;
        }

        public async Task WriteAsync(Document document)
        {
            Console.WriteLine($"Write Web resource .json file... [{_writerConfig.FileName}]");

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
                    if (!ApplyAppFilter(item, _writerConfig.AppsFilter))
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
                            if (key.ns != null)
                            {
                                if (!languageGroup.Namespaces.ContainsKey(key.ns))
                                {
                                    namespaceGroup = new NamespaceGroup();
                                    languageGroup.Namespaces.Add(key.ns, namespaceGroup);
                                }
                                else
                                {
                                    namespaceGroup = languageGroup.Namespaces[key.ns];
                                }
                            }
                            else
                            {
                                namespaceGroup = languageGroup.DefaultNamespace;
                            }

                            namespaceGroup.Translations.Add(key.value, value.Value);
                        }
                    }
                }
            }

            var jDocument = new JObject();

            if (_writerConfig.OmitLanguageObject)
            {
                if (languages.Count > 1)
                {
                    throw new Exception("There are more than 1 language and OmitLanguageObject is true");
                }
            }

            if (languages.Count == 1 && _writerConfig.OmitLanguageObject)
            {
                var language = languages.Single();
                WriteStringsContentToJObject(language, jDocument);
            }
            else
            {
                foreach (var language in languages)
                {
                    var jLanguage = new JObject();
                    WriteStringsContentToJObject(language, jLanguage);
                    jDocument.Add(new JProperty(language.Key.Id, jLanguage));
                }
            }

            using (StreamWriter sw = new StreamWriter(_writerConfig.FileName))
            using (JsonWriter writer = new JsonTextWriter(sw) {Formatting = Formatting.Indented})
            {
                await jDocument.WriteToAsync(writer);
                await sw.WriteLineAsync();
            }
        }

        private JObject WriteStringsContentToJObject(KeyValuePair<Language, LanguageGroup> language, JObject jLanguage)
        {
            foreach (var item in language.Value.DefaultNamespace.Translations)
            {
                jLanguage.Add(new JProperty(item.Key, item.Value));
            }

            foreach (var ns in language.Value.Namespaces)
            {
                var jNs = new JObject();

                jLanguage.Add(ns.Key, jNs);

                foreach (var item in ns.Value.Translations)
                {
                    jNs.Add(new JProperty(item.Key, item.Value));
                }
            }

            return jLanguage;
        }

        private bool ApplyLanguageFilter(Language language)
        {
            return _writerConfig switch
            {
                DestinationResourceWriterConfigMultiLanguage config =>
                    config.LanguageIdsFilter == null || config.LanguageIdsFilter.Contains(language.Id),
                DestinationResourceWriterConfigSingleLanguage config =>
                    config.LanguageId == language.Id,
                _ => throw new NotSupportedException()
            };
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
                var parts = s.Split(':');
                return parts.Length != 2 ? (null, parts[0]) : (parts[0], parts[1]);
            }).ToArray();
        }

        private class LanguageGroup
        {
            public IDictionary<string, NamespaceGroup> Namespaces { get; } = new Dictionary<string, NamespaceGroup>();
            public NamespaceGroup DefaultNamespace { get; } = new NamespaceGroup();
        }

        private class NamespaceGroup
        {
            public IDictionary<string, string> Translations { get; } = new Dictionary<string, string>();
        }
    }
}