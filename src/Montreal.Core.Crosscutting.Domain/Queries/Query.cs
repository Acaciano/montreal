using FluentValidation.Results;
using Montreal.Core.Crosscutting.Common.Data;
using Montreal.Core.Crosscutting.Common.Extensions;
using Montreal.Core.Crosscutting.Domain.Events;
using System;

namespace Montreal.Core.Crosscutting.Domain.Queries
{
    public abstract class Query<TResponse> : QueryMessage<TResponse>
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; } = new ValidationResult();

        protected Query()
        {
            Timestamp = DateTime.Now.ToBrazilianTimezone();
        }

        public abstract bool IsValid();
    }
}