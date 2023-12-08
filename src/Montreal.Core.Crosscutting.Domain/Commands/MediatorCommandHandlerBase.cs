using Montreal.Core.Crosscutting.Domain.Bus;
using Montreal.Core.Crosscutting.Domain.Notifications;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Montreal.Core.Crosscutting.Domain.Commands
{ 
    public abstract class MediatorCommandHandlerBase<TCommand> : AsyncRequestHandler<TCommand>
          where TCommand : Command
    {
        protected IMediatorHandler _mediator { get; }

        protected MediatorCommandHandlerBase(IMediatorHandler mediator)
        {
            _mediator = mediator;
        }

        public abstract Task<bool> AfterValidation(TCommand request);

        protected override Task Handle(TCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);

                return Task.CompletedTask;
            }

            return AfterValidation(request);
        }

        protected void NotifyValidationErrors(TCommand message)
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