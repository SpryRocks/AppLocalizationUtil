using System.Text.Json.Serialization;

namespace AppLocalizationUtil.Data.Loaders.Crowdin.Models
{
    public class DataResponse<T>
    {
        public T Data { get; init; }
    }
}
