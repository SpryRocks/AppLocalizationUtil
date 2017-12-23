using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppLocalizationUtil.Entities;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AppLocalizationUtil.Data.Loaders
{
    public class ExcelFileDocumentReader : IFileDocumentReader
    {

        public string FileName { get; set; }

        private readonly ExcelConfiguration _configuration;

        public ExcelFileDocumentReader(ExcelConfiguration configuration)
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

                var firstRow = sheet.GetRow(sheet.FirstRowNum);
                if (firstRow == null) 
                    continue;

                IDictionary<int, Language> col_lng = new Dictionary<int, Language>();

                foreach (var cell in firstRow.Cells)
                {
                    var value = cell.StringCellValue;
                    if (_configuration.LanguageColumns.ContainsKey(value))
                    {
                        col_lng.Add(cell.ColumnIndex, _configuration.LanguageColumns[value]);
                        continue;
                    }              
                }

                for (int rowNum = firstRow.RowNum + 1; rowNum <= sheet.LastRowNum; rowNum++)
                {
                    var row = sheet.GetRow(rowNum);
                    if (row == null) 
                        break;

                    IDictionary<Language, string> values = new Dictionary<Language, string>();

                    foreach (var cell in row.Cells)
                    {
                        if (!col_lng.ContainsKey(cell.ColumnIndex))
                            continue;

                        var language = col_lng[cell.ColumnIndex];

                        var value = cell.StringCellValue;
                        if (string.IsNullOrWhiteSpace(value))
                            value = null;

                        if (value == null)
                            continue;
                        
                        values.Add(language, value);
                    }

                    if (values.Count < 1)
                        continue;

                    items.Add(new LocalizationItem { Values = values });
                }

                groups.Add(new Group { Name = sheet.SheetName, Items = items });
            }

            return new Document { Groups = groups, Languages = _configuration.LanguageColumns.Values.ToHashSet() };
        }
    }
}