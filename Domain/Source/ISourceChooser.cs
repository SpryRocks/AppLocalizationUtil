using AppLocalizationUtil.Data.Sources;
using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Domain.Source
{
    public interface ISourceChooser
    {
         ISource Choose(JObject sourceConfig);
    }
}