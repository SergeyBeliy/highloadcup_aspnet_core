using Microsoft.EntityFrameworkCore;

namespace AccountsApi.Models {
    public class AccountContext : DbContext {
        public AccountContext (DbContextOptions<AccountContext> options) : base (options) { }
        public DbSet<Account> Accounts { get; set; }

    }
}