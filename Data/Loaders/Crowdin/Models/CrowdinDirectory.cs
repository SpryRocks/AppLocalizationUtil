using System.Text.Json.Serialization;

namespace AppLocalizationUtil.Data.Loaders.Crowdin.Models
{
    public class CrowdinDirectory
    {
        public int Id { get; init; }
        public string Name { get; init; }
    }
}
