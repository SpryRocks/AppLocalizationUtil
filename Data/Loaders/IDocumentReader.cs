using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Loaders
{
    public interface IDocumentReader
    {
         Task<Document> ReadAsync();
    }
}