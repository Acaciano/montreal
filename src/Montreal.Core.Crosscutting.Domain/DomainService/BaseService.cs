using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;
using Montreal.Core.Crosscutting.Common.Entity.Base.V2;
using Montreal.Core.Crosscutting.Domain.Bus;
using Montreal.Core.Crosscutting.Domain.Interfaces.Repositories.V2;
using Montreal.Core.Crosscutting.Domain.Notifications;

namespace Montreal.Core.Crosscutting.Domain.DomainService
{
    public abstract class BaseService<TEntity> : IBaseService where TEntity : BaseEntity, IEntity
    {
        protected readonly IMediatorHandler _mediator;
        protected readonly IOracleRepository<TEntity> _repository;

        public BaseService(IMediatorHandler mediator, IOracleRepository<TEntity> repository)
        {
            this._mediator = mediator;
            this._repository = repository;
        }

        public void NotifyError(string code, string message)
        {
            this._mediator.RaiseEvent(new DomainNotification(code, message));
        }

        public void NotifyError(string message)
        {
            this._mediator.RaiseEvent(new DomainNotification(string.Empty, message));
        }

        public void NotifyError(List<ValidationFailure> validationFailures)
        {
            if (validationFailures != null && validationFailures.Any())
            {
                foreach (ValidationFailure validationFailure in validationFailures)
                {
                    this._mediator.RaiseEvent(new DomainNotification(validationFailure.ErrorCode, validationFailure.ErrorMessage));
                }
            }
        }
    }
}
