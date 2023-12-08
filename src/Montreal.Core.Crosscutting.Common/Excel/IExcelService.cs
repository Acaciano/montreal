using System.Collections.Generic;
using System.IO;

namespace Montreal.Core.Crosscutting.Common.Excel
{
    public interface IExcelService
    {
        byte[] GenerateExcel<TList, TFooter>(IEnumerable<TList> list, TFooter footer, string sheetName = "Planilha");

        byte[] GenerateExcel<TList>(IEnumerable<TList> list, string sheetName = "Planilha");

        List<T> ReadExcel<T>(MemoryStream ms, bool hasHeader = true) where T : new();
    }
}
