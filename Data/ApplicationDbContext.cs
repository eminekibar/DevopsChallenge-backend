using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Burada DbSet'lerinizi tanımlayabilirsiniz, örnek:
        // public DbSet<YourModel> YourModels { get; set; }
    }
}