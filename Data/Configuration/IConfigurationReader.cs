using System.Threading.Tasks;

namespace AppLocalizationUtil.Data.Configuration
{
    public interface IConfigurationReader
    {
        Task<ConfigurationModel> ReadAsync();
    }
}