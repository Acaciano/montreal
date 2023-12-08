using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Montreal.Core.Crosscutting.Common.Data
{
    public interface IPagedList<TEntity> where TEntity : class
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IList<TEntity> Results { get; set; }

        public int FirstRecordOnPage => TotalRecords > 0 ? (CurrentPage - 1) * PageSize + 1 : 0;

        public int LastRecordOnPage => Math.Min(CurrentPage * PageSize, TotalRecords);

        [JsonIgnore]
        public Page Page { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }
    }
}
