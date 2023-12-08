using System.Collections.Generic;

namespace Montreal.Core.Crosscutting.Common.Data
{
    public class PaginationObject
    {
        public Page Page { get; set; }
        public ICollection<Order> Ordenations { get; set; }
        public ICollection<Filter> Filters { get; set; }

        public PaginationObject()
        {
            this.Page = new Page(1, 20);
            this.Ordenations = new List<Order>();
            this.Filters = new List<Filter>();
        }
    }
}