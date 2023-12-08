using Montreal.Core.Crosscutting.Domain.Bus;
using Montreal.Core.Crosscutting.Domain.Commands;
using Montreal.Core.Crosscutting.Domain.Events;
using Montreal.Core.Crosscutting.Domain.Notifications;
using Montreal.Core.Crosscutting.Domain.Queries;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Montreal.Core.Crosscutting.Bus
{
    public sealed class InMemoryBus : IMediatorHandler
    {
        private readonly IMediator _mediator;
        private readonly DomainNotificationHandler _domainNotification;

        public InMemoryBus(
            IMediator mediator,
            INotificationHandler<DomainNotification> domainNotification)
        {
            _mediator = mediator ?? throw new ArgumentException(nameof(mediator));
            _domainNotification = (DomainNotificationHandler)domainNotification ??
                throw new ArgumentException(nameof(domainNotification));
        }

        public IEnumerable<string> Errors => this.GetNotifications().Result.Select(t => t.Value);

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public Task<TResponse> SendQuery<TResponse>(Query<TResponse> query) where TResponse : class
        {
            return _mediator.Send(query);
        }

        public Task RaiseEvent<T>(T @event) where T : Event
        {
            return _mediator.Publish(@event);
        }

        public Task NotifyError(string code, string message)
        {
            return this.RaiseEvent(new DomainNotification(code, message));
        }

        public Task NotifyError(string message)
        {
            return this.NotifyError(string.Empty, message);
        }

        public Task<List<DomainNotification>> GetNotifications()
        {
            return Task.FromResult(this._domainNotification.GetNotifications());
        }

        public Task Clear()
        {
            this._domainNotification.GetNotifications().Clear();
            
            return Task.CompletedTask;
        }

        public INotificationHandler<DomainNotification> GetNotificationHandler()
        {
            return _domainNotification;
        }

        public bool HasNotification()
        {
            return _domainNotification.HasNotifications();
        }
    }
}