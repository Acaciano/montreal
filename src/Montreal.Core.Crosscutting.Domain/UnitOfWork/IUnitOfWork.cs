using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Montreal.Core.Crosscutting.Domain.UnitOfWork
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        Task<bool> CommitAsync();
    }
}
