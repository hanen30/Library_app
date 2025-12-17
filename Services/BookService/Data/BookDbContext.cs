using Microsoft.EntityFrameworkCore;
using BookService.Models;

namespace BookService.Data
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed data pour tester
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Le Petit Prince", Author = "Antoine de Saint-Exupéry", Year = 1943, Genre = "Conte philosophique" },
                new Book { Id = 2, Title = "1984", Author = "George Orwell", Year = 1949, Genre = "Science-fiction" },
                new Book { Id = 3, Title = "Dune", Author = "Frank Herbert", Year = 1965, Genre = "Science-fiction" }
            );
        }
    }
}
