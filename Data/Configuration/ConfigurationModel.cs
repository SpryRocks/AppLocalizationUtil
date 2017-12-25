using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Data.Configuration
{
    public class ConfigurationModel
    {
        public string Project { get; set; }
        public JObject Source { get; set; }
        public string DestinationPath { get; set; }
        public IList<JObject> Destinations { get; set; }
    }
}