using System.Threading.Tasks;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public interface IDestination
    {
         Task WriteAsync(Document document);
    }
}