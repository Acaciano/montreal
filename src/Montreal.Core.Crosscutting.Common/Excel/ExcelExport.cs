using OfficeOpenXml;
using System.Data;
using System.IO;

namespace Montreal.Core.Crosscutting.Common.Excel
{
    public static class ExcelExport
    {
        public static byte[] WriteSpreadsheetToHttpResponse(string name, DataTable data)
        {
            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add(name);
            worksheet.Cells["A1"].LoadFromDataTable(data, true);
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.Row(1).Style.Font.Bold = true;

            MemoryStream stream = new MemoryStream();
            excel.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
