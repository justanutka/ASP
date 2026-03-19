using Microsoft.EntityFrameworkCore;
using UniDesc.Web.Models;

namespace UniDesc.Web
{
	public class UniDeskDbContext : DbContext
	{
		public UniDeskDbContext(DbContextOptions<UniDeskDbContext> options)
			: base(options)
		{
		}

		public required  DbSet<Ticket> Tickets { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Ticket>()
				.Property(t => t.Title)
				.HasMaxLength(60)
				.IsRequired();
		}
	}
}