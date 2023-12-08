using System;
using Montreal.Core.Crosscutting.Common.Exceptions;

namespace Montreal.Core.Crosscutting.Common
{
    internal class FilterNotConfiguredException : PaginationException
    {
        public FilterNotConfiguredException() : base("Filter not configured previously") { }
    }
}
