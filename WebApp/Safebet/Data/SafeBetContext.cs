using Microsoft.EntityFrameworkCore;
using SafeBet.Models;

namespace SafeBet.Data
{
    public class SafeBetContext: DbContext
    {
        public SafeBetContext(DbContextOptions<SafeBetContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Metrics> UserMetrics { get; set; } = null!;

    }
}
