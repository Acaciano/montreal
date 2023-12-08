using System;

namespace Montreal.Core.Crosscutting.Common.Exceptions
{
    internal class PaginationException : Exception
    {
        public PaginationException(string message) : base(message) { }
    }
}
