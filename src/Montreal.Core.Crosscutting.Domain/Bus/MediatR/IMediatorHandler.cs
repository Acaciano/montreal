using Montreal.Core.Crosscutting.Domain.Commands;
using Montreal.Core.Crosscutting.Domain.Events;
using Montreal.Core.Crosscutting.Domain.Notifications;
using Montreal.Core.Crosscutting.Domain.Queries;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Montreal.Core.Crosscutting.Domain.Bus
{
    public interface IMediatorHandler
    {
        Task SendCommand<T>(T command) where T : Command;
        Task<TResponse> SendQuery<TResponse>(Query<TResponse> query) where TResponse : class;
        Task RaiseEvent<T>(T @event) where T : Event;
        Task NotifyError(string code, string message);
        Task NotifyError(string message);
        Task Clear();
        Task<List<DomainNotification>> GetNotifications();
        IEnumerable<string> Errors { get; }
        bool HasNotification();
        INotificationHandler<DomainNotification> GetNotificationHandler();
    }
}