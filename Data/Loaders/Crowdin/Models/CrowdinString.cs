namespace AppLocalizationUtil.Data.Loaders.Crowdin.Models
{
    public class CrowdinString
    {
        public int Id { get; init; }
        public string Identifier { get; init; }
        public string Text { get; init; }
        public int FileId { get; init; }
    }
}
