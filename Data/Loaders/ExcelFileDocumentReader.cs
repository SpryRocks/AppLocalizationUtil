using System.Collections.Generic;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AppLocalizationUtil.Data.Loaders
{
    public class ExcelFileDocumentReader : IFileDocumentReader
    {

        public string FileName { get; set; }

        private readonly Configuration _configuration;

        public ExcelFileDocumentReader(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Task<Document> ReadAsync()
        {
            return Task.Run(() => ReadInternal());
        }

        private Document ReadInternal()
        {
            // TODO: implement

            List<Group> groups = new List<Group>();

            IWorkbook workbook = new XSSFWorkbook(FileName);

            var numberOfSheets = workbook.NumberOfSheets;

            for (int i = 0; i < numberOfSheets; i++)
            {
                List<LocalizationItem> items = new List<LocalizationItem>();

                var sheet = workbook.GetSheetAt(i);

                for (int rowNum = sheet.FirstRowNum + 1; rowNum < 10000; rowNum++) {
                    var row = sheet.GetRow(rowNum);
                    
                    if (row == null) 
                    {
                        break;
                    }

                    var cell = row.GetCell(0);

                    var value = cell.StringCellValue;

                    IDictionary<Language, string> values = new Dictionary<Language, string>();

                    values.Add(new Language { Id = "en", Name = "English" }, value);

                    items.Add(new LocalizationItem { Values = values });
                }

                groups.Add(new Group { Name = sheet.SheetName, Items = items });
            }

            return new Document { Groups = groups };
        }

        public class Configuration
        {
        }
    }
}