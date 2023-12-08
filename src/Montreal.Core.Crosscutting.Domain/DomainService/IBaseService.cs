using FluentValidation.Results;
using System.Collections.Generic;

namespace Montreal.Core.Crosscutting.Domain.DomainService
{
    public interface IBaseService
    {
        void NotifyError(string code, string message);
        void NotifyError(string message);
        void NotifyError(List<ValidationFailure> validationFailures);
    }
}
