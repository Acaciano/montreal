using System;
using System.Collections.Generic;

namespace Montreal.Core.Crosscutting.Common.Data.V2
{
    public interface IPagedList<TType> where TType : class
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IList<TType> Results { get; set; }
        public int FirstRecordOnPage => TotalRecords > 0 ? (CurrentPage - 1) * PageSize + 1 : 0;
        public int LastRecordOnPage => Math.Min(CurrentPage * PageSize, TotalRecords);


        public PagedList<TDestination> ConvertTo<TDestination>(Func<TType, TDestination> converter) where TDestination : class;
       
    }
}
