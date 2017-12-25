using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Destinations
{
    public class AndroidXmlResourceWriter : IDestination
    {
        public string FileName { get; set; }

        public async Task WriteAsync(Document document)
        {
            var en = document.Languages.Where(it => it.Id == "en").Single();

            var xDocument = new XDocument();
            var xResources = new XElement("resources");
            
            xDocument.Add(xResources);

            foreach (var group in document.Groups) 
            {
                foreach (var item in group.Items) 
                {
                    if (item.Apps.Count > 0) 
                        continue;
                    if (!item.Keys.ContainsKey("Android"))
                        continue;
                    if (!item.Values.ContainsKey(en))
                        continue;

                    var key = item.Keys["Android"];
                    var value = item.Values[en];
                    
                    var xStringt = new XElement("string", new XAttribute("name", key));
                    xStringt.Value = PrepareValue(value);
                    xResources.Add(xStringt);
                }
            }

            using(var file = new FileStream(FileName, FileMode.Create))
            {
                await xDocument.SaveAsync(file, SaveOptions.None, CancellationToken.None);
            }
        }

        private string PrepareValue(string value)
        {
            value = value.Replace("?")
        }
    }
}