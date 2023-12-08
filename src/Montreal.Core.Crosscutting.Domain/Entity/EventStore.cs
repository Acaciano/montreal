using System;
using Montreal.Core.Crosscutting.Common.Entity.Base.V1;

namespace Montreal.Core.Crosscutting.Domain.Entity
{
    public class EventStore : BaseEntity
    {
        public string StoreType { get; set; }
        public Guid PersonId { get; set; }
        public string UserName { get; set; }
        public DateTime TimeStamp { get; set; }
        public object Data { get; set; }
    }
}