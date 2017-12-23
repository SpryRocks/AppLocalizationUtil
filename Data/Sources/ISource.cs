using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Sources
{
    public interface ISource
    {
         Task<Document> LoadAsync();
    }
}