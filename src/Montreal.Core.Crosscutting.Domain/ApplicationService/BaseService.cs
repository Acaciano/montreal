using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Domain.Bus;
using Montreal.Core.Crosscutting.Domain.Events;
using Montreal.Core.Crosscutting.Domain.Notifications;
using Montreal.Core.Crosscutting.Domain.UnitOfWork;

namespace Montreal.Core.Crosscutting.Domain.ApplicationService
{
    public abstract class BaseService<TContext> where TContext : DbContext
    {
        protected readonly IMediatorHandler _mediator;
        protected readonly IUnitOfWork<TContext> _unitOfWork;

        public BaseService(IMediatorHandler mediator, IUnitOfWork<TContext> unitOfWork)
        {
            this._mediator = mediator;
            this._unitOfWork = unitOfWork;
        }

        public void NotifyError(string code, string message)
        {
            _mediator.RaiseEvent(new DomainNotification(code, message));
        }

        public void NotifyError(string message)
        {
            _mediator.RaiseEvent(new DomainNotification(string.Empty, message));
        }

        public bool HasNotification()
        {
            return this._mediator.HasNotification();
        }

        public async Task<bool> CommitAsync()
        {
            return await this._unitOfWork.CommitAsync();
        }
        
        public async Task RaiseEvent<T>(T @event) where T : Event
        {
            await this._mediator.RaiseEvent(@event);
        }
    }
}
