using System;

namespace Montreal.Core.Crosscutting.Common.Exceptions
{
    internal class OrdenationNotConfiguredException : PaginationException
    {
        public OrdenationNotConfiguredException() : base("Order not configured previously") { }
    }
}
