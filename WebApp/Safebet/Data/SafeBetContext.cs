using Microsoft.EntityFrameworkCore;
using SafeBet.Models;

namespace SafeBet.Data
{
    public class SafeBetContext: DbContext
    {
        public SafeBetContext (DbContextOptions<SafeBetContext> options) : base(options) { }
        public DbSet<SafeBet.Models.Metrics> Metrics { get; set; } 

    }
}
