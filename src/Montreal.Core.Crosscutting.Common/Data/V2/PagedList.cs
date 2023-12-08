using System;
using System.Collections.Generic;

namespace Montreal.Core.Crosscutting.Common.Data.V2
{
    public class PagedList<T> : IPagedList<T> where T : class
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IList<T> Results { get; set; }
        public int FirstRecordOnPage => TotalRecords > 0 ? (CurrentPage - 1) * PageSize + 1 : 0;
        public int LastRecordOnPage => Math.Min(CurrentPage * PageSize, TotalRecords);


        public PagedList<TDestination> ConvertTo<TDestination>(Func<T, TDestination> converter) where TDestination : class
        {
            PagedList<TDestination> returnValue = new PagedList<TDestination>();

            returnValue.TotalPages = this.TotalPages;
            returnValue.TotalRecords = this.TotalRecords;
            returnValue.PageSize = this.PageSize;
            returnValue.CurrentPage = this.CurrentPage;
            returnValue.Results = new List<TDestination>();

            foreach (T resultItem in this.Results)
            {
                returnValue.Results.Add(converter(resultItem));
            }

            return returnValue;
        }
    }
}
