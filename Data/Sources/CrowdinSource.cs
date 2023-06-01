using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Loaders.Crowdin.Api;
using AppLocalizationUtil.Data.Loaders.Crowdin.Models;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Sources
{
    public class CrowdinSource : ISource
    {
        private readonly Config _config;
        private readonly CrowdinApi _api;

        public CrowdinSource(Config config)
        {
            _config = config;
            _api = new CrowdinApi
            {
                ApiKey = config.ApiKey,
            };
        }

        public async Task<Document> LoadAsync()
        {
            Console.WriteLine(@"Crowdin");

            var groups = new List<Group>();
            var languages = _config.Languages
                .Select(l => new Language {Id = l.Id, Name = l.Name})
                .ToDictionary(l => l.Id);
            var platforms = new HashSet<string>();

            var project = await _api.GetProject(ProjectId);

            var directory = await GetDirectory(_config.Directory);
            var files = (await _api.GetFiles(ProjectId, directory.Id)).ToArray();

            var strings = (await _api.GetStrings(ProjectId, directory.Id)).ToList();
            var fileStrings = strings
                .GroupBy(s => s.FileId)
                .ToDictionary(
                    s => s.Key,
                    s => s as IEnumerable<CrowdinString>
                );
            var languageTranslations = await GetTranslationsAndGroupByLanguage(
                project, languages.Values, strings
            );

            foreach (var file in files)
            {
                var name = GetGroupName(file);

                var defaultLanguage = GetCrowdinLanguageById(project, _config.DefaultLanguageId);

                var currentStrings = fileStrings[file.Id].ToList();

                var items = new List<LocalizationItem>();

                foreach (var crowdinString in currentStrings)
                {
                    var (key, stringPlatforms) = GetStringKeyAndPlatforms(crowdinString);
                    var keys = stringPlatforms.ToDictionary(sp => sp, _ => key);
                    var values = new Dictionary<Language, string>();
                    var apps = new HashSet<string>();

                    values.Add(languages[defaultLanguage.TwoLettersCode], crowdinString.Text);

                    foreach (var language in languages.Values)
                    {
                        if (language.Id == _config.DefaultLanguageId) continue;
                        var currentTranslations =
                            FilterTranslationsByStringIds(languageTranslations[language.Id], currentStrings);
                        var currentTranslation =
                            currentTranslations.FirstOrDefault(t => t.StringId == crowdinString.Id);
                        if (currentTranslation == null) continue;
                        values.Add(language, currentTranslation.Text);
                    }

                    items.Add(new LocalizationItem
                    {
                        Keys = keys,
                        Values = values,
                        Apps = apps,
                    });
                }

                var group = new Group
                {
                    Name = name,
                    Items = items
                };
                groups.Add(group);
            }

            return new Document
            {
                Groups = groups,
                Languages = languages.Values.ToHashSet(),
                Platforms = platforms,
            };
        }

        private async Task<IEnumerable<CrowdinTranslation>>
            GetTranslationsAndFilterByStringIds(CrowdinLanguage language, ICollection<int> stringsIds)
        {
            var translations = await _api.GetTranslations(ProjectId, language.Id);
            return FilterTranslationsByStringIds(translations, stringsIds);
        }

        private async Task<Dictionary<string, IEnumerable<CrowdinTranslation>>> GetTranslationsAndGroupByLanguage(
            CrowdinProject project, IEnumerable<Language> languages, IEnumerable<CrowdinString> strings)
        {
            var stringsIds = strings.Select(s => s.Id).ToHashSet();
            var dictionary = new Dictionary<string, IEnumerable<CrowdinTranslation>>();
            await Parallel.ForEachAsync(languages, async (language,_) =>
            {
                var crowdinLanguage = GetCrowdinLanguageById(project, language.Id);
                var translations = await GetTranslationsAndFilterByStringIds(crowdinLanguage, stringsIds);
                dictionary[language.Id] = translations;
            });

            return dictionary;
        }

        private IEnumerable<CrowdinTranslation> FilterTranslationsByStringIds(
            IEnumerable<CrowdinTranslation> translations,
            IEnumerable<int> stringIds)
        {
            return translations.Where(t => stringIds.Contains(t.StringId));
        }

        private IEnumerable<CrowdinTranslation> FilterTranslationsByStringIds(
            IEnumerable<CrowdinTranslation> translations,
            IEnumerable<CrowdinString> strings)
        {
            var stringsIds = strings.Select(s => s.Id).ToHashSet();
            return FilterTranslationsByStringIds(translations, stringsIds);
        }

        private async Task<CrowdinDirectory> GetDirectory(string directoryName)
        {
            var directories = await _api.GetDirectories(ProjectId);
            var directory = directories.FirstOrDefault(d => d.Name == directoryName);
            if (directory == null) throw new Exception($"Directory with name {directoryName} not found");
            return directory;
        }

        private int ProjectId => _config.ProjectId;

        private string GetGroupName(CrowdinFile file)
        {
            if (file.Title != null) return file.Title;
            var name = file.Name;
            var index = name.LastIndexOf(".", StringComparison.Ordinal);
            return index < 0 ? name : name.Remove(index);
        }

        private (string, string[]) GetStringKeyAndPlatforms(CrowdinString crowdinString)
        {
            var key = crowdinString.Identifier;
            var platforms = new List<string>();

            var startIndex = key.IndexOf("(", StringComparison.Ordinal);
            var endIndex = key.LastIndexOf(")", StringComparison.Ordinal);
            if (startIndex > -1 && endIndex > startIndex)
            {
                var platformsString = key.Substring(startIndex + 1, endIndex - startIndex - 1);
                key = key.Remove(startIndex).Trim();
                platforms.AddRange(platformsString.Split(",", StringSplitOptions.TrimEntries));
            }

            if (platforms.Count < 1)
            {
                platforms.Add(_config.DefaultPlatform);
            }

            return (key, platforms.ToArray());
        }

        private CrowdinLanguage GetCrowdinLanguageById(CrowdinProject project, string languageId)
        {
            var configLanguage = _config.Languages.FirstOrDefault(l => l.Id == languageId);
            if (configLanguage is {CrowdinId: not null})
            {
                languageId = configLanguage.CrowdinId;
            }

            var language = project.TargetLanguages.FirstOrDefault(l => l.TwoLettersCode == languageId);
            if (language == null) throw new Exception($"Language not found {languageId}");
            return language;
        }

        public void Dispose()
        {
        }
    }

    public class Config
    {
        public string ApiKey { get; init; }
        public int ProjectId { get; init; }
        public string Directory { get; init; }
        public Language[] Languages { get; init; }
        public string DefaultLanguageId { get; init; }
        public string DefaultPlatform { get; init; }

        public class Language
        {
            public string Id { get; init; }
            public string Name { get; init; }
            public string CrowdinId { get; init; }
        }
    }
}