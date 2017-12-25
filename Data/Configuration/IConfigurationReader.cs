using System.Threading.Tasks;
using AppLocalizationUtil.Data.Configuration.Model;

namespace AppLocalizationUtil.Data.Configuration
{
    public interface IConfigurationReader
    {
        Task<ConfigurationModel> ReadAsync();
    }
}