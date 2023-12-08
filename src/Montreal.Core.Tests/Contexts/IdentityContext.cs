using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Montreal.Core.Tests.Contexts
{
    public class IdentityContext : IdentityDbContext
    {
        public IdentityContext()
        {
        }

        public IdentityContext(DbContextOptions<IdentityContext> opt) : base(opt)
        {
        }
    }
}
