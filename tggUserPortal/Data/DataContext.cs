using Microsoft.EntityFrameworkCore;
using tggUserPortal.Model;

namespace tggUserPortal.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<UsersDto> Users { get; set; }
    }
}
