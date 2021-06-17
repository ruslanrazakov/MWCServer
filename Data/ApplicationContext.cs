using Microsoft.EntityFrameworkCore;
using MWCServer.Models;

namespace Data.MWCServer
{
    public class ApplicationContext : DbContext
    {
         public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            
        }

        public DbSet<Photo> Photos { get; set; }
    }
}