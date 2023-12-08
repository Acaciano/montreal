using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Domain.UnitOfWork;

namespace Montreal.Core.Crosscutting.Infrastructure.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly TContext _context;

        public UnitOfWork(TContext context)
        {
            _context = context;
        }

        public async Task<bool> CommitAsync()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool success = (await _context.SaveChangesAsync()) > 0;

                if (success)
                    await transaction.CommitAsync();
                else
                    await transaction.RollbackAsync();

                return success;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
