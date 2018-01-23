using System.Collections.Generic;
using AppLocalizationUtil.Data.Destinations;
using Newtonsoft.Json.Linq;

namespace AppLocalizationUtil.Domain.Destination
{
    public interface IDestinationChooser
    {
        IList<IDestination> Choose(IList<JObject> destinationConfigs);
    }
}