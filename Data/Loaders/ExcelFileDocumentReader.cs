using System;
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
            Console.WriteLine($"Read excel document... [{FileName}]");

            IWorkbook workbook = null;
            try 
            {
                workbook = new XSSFWorkbook(FileName);
                return ReadWorkbook(workbook);
            }
            finally
            {
                if (workbook != null) 
                {
                    workbook.Close();
                }
            }
        }

        private Document ReadWorkbook(IWorkbook workbook)
        {
            List<Group> groups = new List<Group>();

            var numberOfSheets = workbook.NumberOfSheets;

            for (int i = 0; i < numberOfSheets; i++)
            {
                List<LocalizationItem> items = new List<LocalizationItem>();

                var sheet = workbook.GetSheetAt(i);

                var firstRow = sheet.GetRow(sheet.FirstRowNum);
                if (firstRow == null) 
                    continue;

                IDictionary<int, Language> col_lng = new Dictionary<int, Language>();
                IDictionary<int, string> col_key = new Dictionary<int, string>();
                int? col_apps = null;

                foreach (var cell in firstRow.Cells)
                {
                    var value = cell.StringCellValue;
                    var columnIndex = cell.ColumnIndex;
                    
                    if (_configuration.LanguageColumns.ContainsKey(value))
                    {
                        col_lng.Add(columnIndex, _configuration.LanguageColumns[value]);
                        continue;
                    }

                    if (_configuration.PlatformKeyColumns.ContainsKey(value))
                    {
                        col_key.Add(columnIndex, _configuration.PlatformKeyColumns[value]);
                        continue;
                    }

                    if (_configuration.AppsColumn == value)
                    {
                        col_apps = columnIndex;
                        continue;
                    }
                }

                for (int rowNum = firstRow.RowNum + 1; rowNum <= sheet.LastRowNum; rowNum++)
                {
                    var row = sheet.GetRow(rowNum);
                    if (row == null) 
                        break;

                    IDictionary<Language, string> values = new Dictionary<Language, string>();
                    IDictionary<string, string> keys = new Dictionary<string, string>();
                    ISet<string> apps = null;

                    foreach (var cell in row.Cells)
                    {
                        var value = cell.StringCellValue;
                        if (string.IsNullOrWhiteSpace(value))
                            value = null;

                        if (value == null)
                            continue;

                        int columnIndex = cell.ColumnIndex;
                        
                        if (col_lng.ContainsKey(columnIndex))
                        {
                            var language = col_lng[columnIndex];

                            values.Add(language, value);
                            
                            continue;
                        }

                        if (col_key.ContainsKey(columnIndex))
                        {
                            var key = col_key[columnIndex];

                            keys.Add(key, value);

                            continue;
                        }

                        if (col_apps == columnIndex)
                        {
                             apps = value.Split(",").Select(it => it.Trim()).ToHashSet();

                             continue;
                        }
                    }

                    if (values.Count < 1)
                        continue;

                    if (apps == null)
                        apps = new HashSet<string>();

                    items.Add(new LocalizationItem { Values = values, Keys = keys, Apps = apps });
                }

                groups.Add(new Group { Name = sheet.SheetName, Items = items });
            }

            return new Document
            {
                Groups = groups, 
                Languages = _configuration.LanguageColumns.Values.ToHashSet(),
                Platforms = _configuration.PlatformKeyColumns.Values.ToHashSet()
            };
        }
    }
}