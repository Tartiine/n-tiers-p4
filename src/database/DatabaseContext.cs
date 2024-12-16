using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Host)
                .WithMany()
                .HasForeignKey(g => g.HostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Guest)
                .WithMany()
                .HasForeignKey(g => g.GuestId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Player>().HasData(
                new Player { Id = 1, Login = "testuser", Password = "testpassword" },
                new Player { Id = 2, Login = "guestuser", Password = "guestpassword" }
            );

            modelBuilder.Entity<Game>().HasData(
                new Game { Id = 1, HostId = 1, GuestId = 2, Status = "AwaitingGuest" }
            );
        }
    }
}
