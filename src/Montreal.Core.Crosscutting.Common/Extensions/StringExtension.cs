using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Montreal.Core.Crosscutting.Common.Extensions
{
    public static class StringExtension
    {
        public static string RemoveAccents(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            StringBuilder sbReturn = new();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }

        public static string OnlyNumbers(this string regex) => regex != null ? Regex.Replace(regex, @"\D", "") : regex;

        public static string GetFirstName(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            string[] arrayValue = value.Split(' ');

            return arrayValue != null ? arrayValue.FirstOrDefault() : value;
        }

        public static string GetStringWithoutAccentsSpecialCharacters(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            /** Replace accented characters with unaccented characters **/
            string[] accents = new string[] { "ç", "Ç", "á", "é", "í", "ó", "ú", "ý", "Á", "É", "Í", "Ó", "Ú", "Ý", "à", "è", "ì", "ò", "ù", "À", "È", "Ì", "Ò", "Ù", "ã", "õ", "ñ", "ä", "ë", "ï", "ö", "ü", "ÿ", "Ä", "Ë", "Ï", "Ö", "Ü", "Ã", "Õ", "Ñ", "â", "ê", "î", "ô", "û", "Â", "Ê", "Î", "Ô", "Û" };
            string[] wihoutAccent = new string[] { "c", "C", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "Y", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U", "a", "o", "n", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "A", "O", "N", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U" };

            for (int i = 0; i < accents.Length; i++)
            {
                str = str.Replace(accents[i], wihoutAccent[i]);
            }
            /** Replaces the special characters of the string with **/
            string[] especialCharacters = { "¹", "²", "³", "£", "¢", "¬", "º", "¨", "\"", "'", ".", ",", "-", ":", "(", ")", "ª", "|", "\\\\", "°", "_", "@", "#", "!", "$", "%", "&", "*", ";", "/", "<", ">", "?", "[", "]", "{", "}", "=", "+", "§", "´", "`", "^", "~" };

            for (int i = 0; i < especialCharacters.Length; i++)
            {
                str = str.Replace(especialCharacters[i], "");
            }

            /** Replaces the special characters of the string with " " **/
            //str = Regex.Replace(str, @"[^\w\.@-]", " ", RegexOptions.None, TimeSpan.FromSeconds(1.5));

            return str.Trim();
        }

        public static string ToUpperTrim(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return value.ToUpper().Trim();
        }

        public static string GetSearchField(this object entity)
        {
            if (entity == null) return string.Empty;

            var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType != typeof(Guid) && !x.Name.Equals("SmartSearch") && x.GetValue(entity) != null); //DeclareOnly not from inherited class
            var propValues = new List<object>();

            foreach (var propInfo in properties)
            {
                var value = propInfo.PropertyType == typeof(string) ? propInfo.GetValue(entity).ToString().GetStringWithoutAccentsSpecialCharacters().ToUpperTrim()
                    : propInfo.GetValue(entity);

                propValues.Add(value);
            }

            return string.Join(" ", propValues);
        }

        public static Expression<Func<TEntity, object>> ToLambdaExpression<TEntity>(this string str)
        {
            return str?.ToLambdaExpression<TEntity, object>();
        }

        public static Expression<Func<TEntity, TType>> ToLambdaExpression<TEntity, TType>(this string str)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str)) return null;

                var parameter = Expression.Parameter(typeof(TEntity));
                var propertyExpression = str.ToMemberExpression<TEntity>(parameter);

                var expressionConvertedToType = Expression.Convert(propertyExpression, typeof(TType));
                return Expression.Lambda<Func<TEntity, TType>>(expressionConvertedToType, parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static string[] AddCustomSelectProperties<TEntity>(this string[] strings, params Expression<Func<TEntity, object>>[] customSelects)
        {
            try
            {
                var fieldsList = strings.ToList();

                foreach (var customSelectProperties in customSelects.Select(x => x.StripConvert().Body.ToString().Split(".").Skip(1).ToArray()))
                {
                    var newField = string.Join(".", customSelectProperties);

                    if (newField.Contains(" ")) throw new Exception("Cannot use anonymous type for custom select");

                    fieldsList.Add(newField);
                }

                return fieldsList.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return strings;
            }
        }

        public static Expression<Func<TEntity, TType>> ToLambdaExpression<TEntity, TType>(this string[] strings)
        {
            try
            {
                if (strings == null || !strings.Any()) return null;

                var parameter = Expression.Parameter(typeof(TEntity));
                var propertyExpression = strings.ToMemberExpression<TEntity>(parameter);

                var expressionConvertedToType = Expression.Convert(propertyExpression, typeof(TType));
                return Expression.Lambda<Func<TEntity, TType>>(expressionConvertedToType, parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static MemberExpression ToMemberExpression<TEntity>(this string str, ParameterExpression initialParameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str)) return null;

                var lastExpression = (MemberExpression)null;

                foreach (var splitedProperty in str.Split("."))
                {
                    var property = splitedProperty.ToTitleCase();
                    lastExpression = Expression.Property(lastExpression == null ? initialParameter : lastExpression, property);
                }

                return lastExpression;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static MemberExpression ToMemberExpression<TEntity>(this string[] strings, ParameterExpression initialParameter)
        {
            try
            {
                if (strings == null || !strings.Any()) return null;

                var lastExpression = (MemberExpression)null;

                foreach (var splitedProperty in strings)
                {
                    var property = splitedProperty.ToTitleCase();
                    lastExpression = Expression.Property(lastExpression == null ? initialParameter : lastExpression, property);
                }

                return lastExpression;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static PropertyInfo GetProperty<TEntity>(this string str)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str)) return null;

                var lastType = typeof(TEntity);
                var properties = str.Split(".");

                foreach (var splitedProperty in properties)
                {
                    var lastTypeProperty = lastType.GetProperties().FirstOrDefault(x => x.Name.ToLower() == splitedProperty.ToLower());

                    if (lastType == null || lastTypeProperty == null) return null;
                    if (lastTypeProperty.Name.ToLower() == properties.Last().ToLower()) return lastTypeProperty;

                    lastType = lastTypeProperty.PropertyType;
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static Expression<Func<TEntity, TType>> GetPropertyToInclude<TEntity, TType>(this string str)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str)) return null;

                var properties = str.Split(".").ToList();
                properties.Remove(properties.Last());

                if (properties.Count() == 0) return null;

                var parameter = Expression.Parameter(typeof(TEntity));
                var propertyExpression = properties.ToArray().ToMemberExpression<TEntity>(parameter);

                return Expression.Lambda<Func<TEntity, TType>>(propertyExpression, parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static string ToTitleCase(this string str)
        {
            var culture = new CultureInfo("en-US", false).TextInfo;
            return str != null ? culture.ToTitleCase(str) : null;
        }

        public static string ConvertToCustomCultureDateString(this string src, CultureInfo cultureInfo = null)
        {
            try
            {
                var (dayPosition, monthPosition, yearPosition) = GetDatePositions(cultureInfo);

                var splited = src.Split("/");

                if (splited.Count() == 1) splited = src.Split(".");
                if (splited.Count() == 1) splited = src.Split("-");

                if (splited.Count() <= 1) return src;

                var hasDay = splited.Count() > dayPosition && !string.IsNullOrWhiteSpace(splited[dayPosition]);
                var hasMonth = splited.Count() > monthPosition && !string.IsNullOrWhiteSpace(splited[monthPosition]);
                var hasYear = splited.Count() > yearPosition && !string.IsNullOrWhiteSpace(splited[yearPosition]);

                var dayString = hasDay ? splited[dayPosition] : String.Empty;
                var monthString = hasMonth ? splited[monthPosition] : String.Empty;
                var yearString = hasYear ? splited[yearPosition] : String.Empty;

                if (hasYear)
                {
                    var actualYear = DateTime.UtcNow.Year.ToString();
                    var firstDigitsOfYear = actualYear.Substring(0, 2);

                    yearString = yearString.Length != actualYear.Length ? $"{firstDigitsOfYear}{yearString}" : yearString;
                }

                var defaultDateValue = "1";
                int.TryParse(hasDay ? dayString : defaultDateValue, out var day);
                int.TryParse(hasMonth ? monthString : defaultDateValue, out var month);
                int.TryParse(hasYear ? yearString : defaultDateValue, out var year);

                var dateTime = new DateTime(year, month, day);

                var dateToCompare = new StringBuilder();

                if (hasDay) dateToCompare.Append(dateTime.ToString("dd"));
                if (hasMonth) dateToCompare.Append($"-{dateTime.ToString("MMM")}");
                if (hasYear) dateToCompare.Append($"-{dateTime.ToString("yy")}");

                return dateToCompare.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return src;
            }
        }

        private static (int, int, int) GetDatePositions(CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null) cultureInfo = CultureInfo.InvariantCulture;

            var date = DateTime.UtcNow.ToString(cultureInfo.DateTimeFormat.ShortDatePattern);

            var dateSplited = date.Split(cultureInfo.DateTimeFormat.DateSeparator);

            var dayToString = DateTime.UtcNow.Day.ToString("00");
            var monthToString = DateTime.UtcNow.Month.ToString("00");
            var yearToString = DateTime.UtcNow.Year.ToString("0000");

            var dayPosition = Array.IndexOf(dateSplited, dayToString);
            var monthPosition = Array.IndexOf(dateSplited, monthToString);
            var yearPosition = Array.IndexOf(dateSplited, yearToString);

            return (dayPosition, monthPosition, yearPosition);
        }
    }
}
