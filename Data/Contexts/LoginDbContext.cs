using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Trackbox.Data
{
    public class LoginDbContext : IdentityDbContext
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> options)
            : base(options)
        {
        }
    }
}