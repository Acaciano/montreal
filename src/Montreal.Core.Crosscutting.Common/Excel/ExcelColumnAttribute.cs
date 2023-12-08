using System;

namespace Montreal.Core.Crosscutting.Common.Excel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnAttribute : Attribute
    {
        private readonly string _columnName;
        private readonly string _comment;
        private readonly int _order;
        private readonly ExcelColumnFormat _format;

        public ExcelColumnAttribute(string columnName, int order, string comment = "", ExcelColumnFormat format = ExcelColumnFormat.General)
        {
            _columnName = columnName;
            _comment = comment;
            _order = order;
            _format = format;
        }

        public string GetColumnName() => _columnName;
        public string GetComment() => _comment;
        public int GetColumnOrder() => _order;
        public string GetFormat()
        {
            switch (_format)
            {
                case ExcelColumnFormat.General: return "General";
                case ExcelColumnFormat.Number: return "0";
                case ExcelColumnFormat.NumberDotTwoDigits: return "0.00";
                case ExcelColumnFormat.Currency: return "R$ #,##0.00;-[Red]R$ #,##0.00";
                case ExcelColumnFormat.Date: return "dd/MM/yyyy";
                case ExcelColumnFormat.Time: return "HH:mm:ss";
                case ExcelColumnFormat.DateTime: return "dd/MM/yyyy - HH:mm:ss";
                case ExcelColumnFormat.Percentage: return "0.00%";
                case ExcelColumnFormat.Text: return "@";
                default: return null;
            }
        }


        public enum ExcelColumnFormat
        {
            General,
            Number,
            NumberDotTwoDigits,
            Currency,
            Date,
            Time,
            DateTime,
            Percentage,
            Text
        }
    }
}
