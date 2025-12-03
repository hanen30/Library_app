using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;

namespace LibraryApp.Data
{
    public class LibraryDbContext : IdentityDbContext<IdentityUser>
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Genre).HasMaxLength(50);
            });

            // Configuration de Member
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuration de Loan
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LoanDate).IsRequired();
                
                entity.HasOne(e => e.Book)
                    .WithMany(b => b.Loans)
                    .HasForeignKey(e => e.BookId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Member)
                    .WithMany(m => m.Loans)
                    .HasForeignKey(e => e.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

