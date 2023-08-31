using Microsoft.EntityFrameworkCore;
namespace SignalROnline.Models
{
	public class ApplicationDbContext : DbContext 
	{
		public DbSet<User> Users { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}
}
