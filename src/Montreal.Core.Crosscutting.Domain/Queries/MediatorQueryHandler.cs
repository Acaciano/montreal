using Montreal.Core.Crosscutting.Domain.Bus;
using Montreal.Core.Crosscutting.Domain.Notifications;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Montreal.Core.Crosscutting.Domain.Queries
{ 
    public abstract class MediatorQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
          where TQuery : Query<TResponse>
          where TResponse : class
    {
        protected IMediatorHandler _mediator { get; }

        protected MediatorQueryHandler(IMediatorHandler mediator)
        {
            _mediator = mediator;
        }

        public abstract Task<TResponse> AfterValidation(TQuery request);

        public Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);

                return Task.FromResult<TResponse>(null);
            }

            return AfterValidation(request);
        }

        protected void NotifyValidationErrors(TQuery message)
        {
            foreach (var error in message.ValidationResult.Errors)
            {
                _mediator.RaiseEvent(new DomainNotification(message.MessageType, error.ErrorMessage));
            }
        }

        protected void NotifyError(string code, string message) => this._mediator.NotifyError(code, message);
        protected void NotifyError(string message) => this._mediator.NotifyError(string.Empty, message);

        protected void NotifyError(IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                this.NotifyError(message);
            }
        }

        protected bool HasNotification() => this._mediator.HasNotification();
        protected IEnumerable<string> Errors => this._mediator.Errors;
    }
}