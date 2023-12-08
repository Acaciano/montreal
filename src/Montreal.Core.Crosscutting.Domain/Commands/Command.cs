using FluentValidation.Results;
using Newtonsoft.Json;
using Montreal.Core.Crosscutting.Common.Extensions;
using Montreal.Core.Crosscutting.Domain.Events;
using System;

namespace Montreal.Core.Crosscutting.Domain.Commands
{
    public abstract class Command : CommandMessage
    {
        [JsonIgnore()]
        public DateTime Timestamp { get; private set; }

        [JsonIgnore()]
        public ValidationResult ValidationResult { get; set; } = new ValidationResult();

        protected Command()
        {
            Timestamp = DateTime.Now.ToBrazilianTimezone();
        }

        public abstract bool IsValid();
    }
}