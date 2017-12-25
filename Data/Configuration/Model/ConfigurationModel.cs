using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Data.Configuration.Model
{
    public class ConfigurationModel
    {
        public string Project { get; set; }
        public JObject Source { get; set; }
    }
}