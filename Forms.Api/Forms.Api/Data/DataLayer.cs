using Forms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Forms.Api.Data
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}