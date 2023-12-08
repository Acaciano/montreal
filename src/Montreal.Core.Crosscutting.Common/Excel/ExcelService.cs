using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Montreal.Core.Crosscutting.Common.Excel
{
    public class ExcelService : IExcelService
    {
        public ExcelService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public List<T> ReadExcel<T>(MemoryStream ms, bool hasHeader = true) where T : new()
        {
            using (ExcelPackage package = new ExcelPackage(ms))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets[0];
                DataTable dt = ExcelWorksheetToDataTable(ws, hasHeader);

                var result = new List<T>();
                var excelStructure = GetExcelStructure<T>();
                var columnsNames = excelStructure.Select(x => x.ColumnName).ToArray();

                foreach (DataRow row in dt.Rows)
                {
                    var obj = new T();

                    var rowsEmpty = 0;

                    foreach (var item in row.ItemArray)
                    {
                        if (!item.GetType().GetProperties().Any() || string.IsNullOrWhiteSpace(item.ToString()))
                        {
                            rowsEmpty++;
                        }
                    }

                    if (rowsEmpty == row.ItemArray.Length)
                        continue;

                    foreach (var propertyT in obj.GetType().GetProperties())
                    {
                        if (!Attribute.GetCustomAttributes(propertyT).Any(x => x is ExcelColumnAttribute))
                            continue;

                        var structure = excelStructure.First(x => x.PropertyName == propertyT.Name);

                        object value = null;

                        if (structure.Type == typeof(string))
                        {
                            value = row[structure.ColumnName] as string;
                        }
                        else if (structure.Type == typeof(int) || structure.Type == typeof(int?))
                        {
                            value = Convert.ToInt32(row[structure.ColumnName]);
                        }
                        else if (structure.Type == typeof(long) || structure.Type == typeof(long?))
                        {
                            value = Convert.ToInt64(row[structure.ColumnName]);
                        }
                        else if (structure.Type == typeof(float) || structure.Type == typeof(float?))
                        {
                            value = float.Parse(row[structure.ColumnName].ToString());
                        }
                        else if (structure.Type == typeof(decimal) || structure.Type == typeof(decimal?))
                        {
                            value = decimal.Parse(row[structure.ColumnName].ToString());
                        }
                        else if (structure.Type == typeof(double) || structure.Type == typeof(double?))
                        {
                            value = double.Parse(row[structure.ColumnName].ToString());
                        }
                        else if (structure.Type == typeof(bool) || structure.Type == typeof(bool?))
                        {
                            value = bool.Parse(row[structure.ColumnName].ToString());
                        }
                        else if (structure.Type == typeof(DateTime) || structure.Type == typeof(DateTime?))
                        {
                            value = Convert.ToDateTime(row[structure.ColumnName].ToString());
                        }

                        if (value != null)
                        {
                            propertyT.SetValue(obj, value);
                        }
                    }

                    result.Add(obj);
                }

                return result;
            }
        }

        public byte[] GenerateExcel<TList>(IEnumerable<TList> list, string sheetName = "Planilha")
        {
            return this.GenerateExcel<TList, object>(list, null, sheetName);
        }

        public byte[] GenerateExcel<TList, TFooter>(IEnumerable<TList> list, TFooter footer, string sheetName = "Planilha")
        {
            byte[] result = null;

            var excelStructure = GetExcelStructure<TList>();
            var columnsName = excelStructure.Select(x => x.ColumnName).ToArray();

            using (ExcelPackage package = new ExcelPackage())
            {
                var columns = GetColumns(list.FirstOrDefault());
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                for (int i = 0; i < columns.Count(); i++)
                {
                    worksheet.Cells[1, i + 1].Value = columns[i];
                    var comment = excelStructure.FirstOrDefault(w => w.ColumnName == columns[i]).Comment;
                    if (!string.IsNullOrWhiteSpace(comment))
                        worksheet.Cells[1, i + 1].AddComment(comment, "TSystems").AutoFit = true;
                }

                var j = 2;
                var count = 1;

                foreach (var item in list)
                {
                    this.InsertLine<TList>(item, excelStructure, worksheet, j);

                    j++;
                    count++;
                }

                if (footer != null)
                {
                    j += 2;
                    count += 2;

                    this.InsertLine<TFooter>(footer, excelStructure, worksheet, j);
                }

                result = package.GetAsByteArray();
            }

            return result;
        }

        #region Privates
        private void InsertLine<TItem>(TItem item, List<ExcelStructure> excelStructure, ExcelWorksheet worksheet, int rowNumber)
        {
            var properties = item.GetType().GetProperties();
            object[] values = GetOrderedValues(excelStructure, item, properties);

            int cellNumber = 1;

            for (int y = 0; y < properties.Count(); y++)
            {
                if (!Attribute.GetCustomAttributes(properties[y]).Any(x => x is ExcelColumnAttribute))
                    continue;

                if (values[y] != null)
                {
                    if (properties[y].PropertyType == typeof(DateTime) || properties[y].PropertyType == typeof(DateTime?))
                    {
                        worksheet.Cells[rowNumber, cellNumber].Value = Convert.ToDateTime(values[y]).ToShortDateString();
                    }
                    else if (properties[y].PropertyType == typeof(int) || properties[y].PropertyType == typeof(long) || properties[y].PropertyType == typeof(double) || properties[y].PropertyType == typeof(decimal)) 
                    {
                        worksheet.Cells[rowNumber, cellNumber].Value = values[y];
                    }
                    else
                    {
                        worksheet.Cells[rowNumber, cellNumber].Value = values[y]?.ToString();
                    }
                }

                cellNumber++;
            }
        }

        private object[] GetOrderedValues<T>(List<ExcelStructure> excelStructure, T item, PropertyInfo[] properties)
        {
            properties = item.GetType().GetProperties();
            var orderValues = new Dictionary<int, object>();

            foreach (var property in properties)
            {
                if (!Attribute.GetCustomAttributes(property).Any(x => x is ExcelColumnAttribute))
                    continue;

                var orderProperty = excelStructure.First(x => x.PropertyName == property.Name).Order;
                var value = property.GetValue(item);
                orderValues.Add(orderProperty, value);
            }

            return orderValues.OrderBy(x => x.Key).Select(x => x.Value).ToArray();
        }


        private List<string> GetColumns<T>(T data)
        {
            List<string> lst = new List<string>();
            var excelStructure = GetExcelStructure<T>();

            if (data == null) return new List<string>();

            PropertyInfo[] properties = data.GetType().GetProperties();
            var orderValues = new Dictionary<int, string>();

            for (int y = 0; y < properties.Count(); y++)
            {
                if (!Attribute.GetCustomAttributes(properties[y]).Any(x => x is ExcelColumnAttribute))
                    continue;

                var prop = properties[y];
                var structure = excelStructure.FirstOrDefault(x => x.PropertyName == prop.Name);

                if (structure == null) continue;

                var orderProperty = structure.Order;

                orderValues.Add(orderProperty, structure.ColumnName);
            }

            return orderValues.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }

        private DataTable ExcelWorksheetToDataTable(ExcelWorksheet ws, bool hasHeader = true)
        {
            DataTable dt = new DataTable();

            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
            {
                if (!string.IsNullOrWhiteSpace(firstRowCell.Text))
                {
                    string firstColumn = $"Column {firstRowCell.Start.Column}";
                    dt.Columns.Add(hasHeader ? firstRowCell.Text : firstColumn);
                }
            }

            var startRow = hasHeader ? 2 : 1;

            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
                var wsRow = ws.Cells[rowNum, 1, rowNum, dt.Columns.Count];
                DataRow row = dt.Rows.Add();
                foreach (var cell in wsRow)
                {
                    row[cell.Start.Column - 1] = cell.Text;
                }
            }

            return dt;
        }

        private static List<ExcelStructure> GetExcelStructure<T>()
        {
            var result = new List<ExcelStructure>();

            foreach (var property in typeof(T).GetProperties())
            {
                var attributes = Attribute.GetCustomAttributes(property).Where(x => x is ExcelColumnAttribute).ToList();
                foreach (var attribute in attributes)
                {
                    var excelColumnAttribute = (ExcelColumnAttribute)attribute;
                    result.Add(new ExcelStructure
                    {
                        Type = property.PropertyType,
                        PropertyName = property.Name,
                        ColumnName = excelColumnAttribute.GetColumnName(),
                        Comment = excelColumnAttribute.GetComment(),
                        Order = excelColumnAttribute.GetColumnOrder(),
                        Format = excelColumnAttribute.GetFormat()
                    });
                }
            }

            return result.OrderBy(x => x.Order).ToList();
        }

        class ExcelStructure
        {
            public Type Type { get; set; }
            public string PropertyName { get; set; }
            public string ColumnName { get; set; }
            public int Order { get; set; }
            public string Format { get; set; }
            public string Comment { get; set; }
        }

        #endregion
    }
}
