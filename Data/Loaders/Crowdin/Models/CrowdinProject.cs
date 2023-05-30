namespace AppLocalizationUtil.Data.Loaders.Crowdin.Models
{
    public class CrowdinProject
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public CrowdinLanguage[] TargetLanguages { get; init; }
    }
}
