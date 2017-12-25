using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Data.Configuration.Model
{
    public class DestinationsModel
    {
        public string Path { get; set; }
        public IList<JObject> Platforms { get; set; }
    }
}