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

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Ticket ticket)
                {
                    if (entry.State == EntityState.Added)
                    {
                        ticket.CreatedAt = DateTime.UtcNow;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        ticket.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}