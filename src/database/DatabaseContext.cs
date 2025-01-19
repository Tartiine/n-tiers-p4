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
        public DbSet<Grid> Grids { get; set; }
        public DbSet<Cell> Cells { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cell>()
                .HasOne(c => c.Grid)
                .WithMany(g => g.Cells)
                .HasForeignKey(c => c.GridId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Grid>()
                .HasMany(g => g.Cells)
                .WithOne(c => c.Grid)
                .HasForeignKey(c => c.GridId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.Games)
                .WithOne(g => g.Host)
                .HasForeignKey(g => g.HostId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Game>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Grid)
                .WithMany()
                .HasForeignKey(g => g.GridId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Host)
                .WithMany()
                .HasForeignKey(g => g.HostId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Guest)
                .WithMany()
                .HasForeignKey(g => g.GuestId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Game>()
                .Property(g => g.Status)
                .HasConversion<string>();

            // Seed Data
            modelBuilder.Entity<Grid>().HasData(new Grid { Id = 1, Rows = 6, Columns = 7 });

            modelBuilder.Entity<Cell>().HasData(
                Enumerable.Range(0, 6)
                    .SelectMany(row => Enumerable.Range(0, 7)
                    .Select(col => new Cell { Id = (row * 7) + col + 1, Row = row, Column = col, GridId = 1 }))
                    .ToArray()
            );

            modelBuilder.Entity<Player>().HasData(
                new Player { Id = 1, Login = "testuser", Password = "testpassword" },
                new Player { Id = 2, Login = "guestuser", Password = "guestpassword" }
            );

            modelBuilder.Entity<Game>().HasData(
                new Game { Id = 1, GridId = 1, HostId = 1, Status = GameStatus.AwaitingGuest }
            );
        }

    }
}
