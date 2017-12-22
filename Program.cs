using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace AppLocalizationUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            using (var client = new HttpClient())
            {
                var resId = "";
                var authKey = "";
                var url = $"https://onedrive.live.com/download?resid={resId}&authkey={authKey}";
                
                using (var result = await client.GetStreamAsync(url))
                {
                    using(var file = new FileStream(FilePath, FileMode.Create))
                    {
                        await result.CopyToAsync(file);
                    }
                }
            }

            IWorkbook workbook = new XSSFWorkbook(FilePath);

            var numberOfSheets = workbook.NumberOfSheets;

            for (int i = 0; i < numberOfSheets; i++)
            {
                var sheet = workbook.GetSheetAt(i);

                for (int rowNum = sheet.FirstRowNum + 1; rowNum < 10000; rowNum++) {
                    var row = sheet.GetRow(rowNum);
                    
                    if (row == null) 
                    {
                        Console.WriteLine("---");
                        break;
                    }

                    var cell = row.GetCell(0);

                    var value = cell.StringCellValue;

                    Console.WriteLine(value);
                }
            }
        }

        static string FilePath
        {
            get { return Environment.CurrentDirectory + "/wrk.xlsx"; }
        }
    }
}
