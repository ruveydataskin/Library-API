using System;
using LibraryAPI6.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI6.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        // DbSets for various entities in the database
        public DbSet<Book>? Books { get; set; }
        public DbSet<Location>? Locations { get; set; }
        public DbSet<Language>? Languages { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<SubCategory>? SubCategories { get; set; }
        public DbSet<Publisher>? Publishers { get; set; }
        public DbSet<Author>? Authors { get; set; }
        public DbSet<AuthorBook>? AuthorBooks { get; set; }
        public DbSet<BookLanguage>? BookLanguages { get; set; }
        public DbSet<BookSubCategory>? BookSubCategories { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<Member>? Members { get; set; }
        public DbSet<BookCopy>? BookCopies { get; set; }
        public DbSet<Loan>? Loans { get; set; }
        public DbSet<Admin>? Admins { get; set; }
        public DbSet<Rating>? Ratings { get; set; }

        // Configures the model relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite primary keys for many-to-many relationships
            modelBuilder.Entity<AuthorBook>().HasKey(a => new { a.AuthorsId, a.BooksId });
            modelBuilder.Entity<BookLanguage>().HasKey(a => new { a.BooksId, a.Code });
            modelBuilder.Entity<BookSubCategory>().HasKey(a => new { a.BooksId, a.Id });
        }

        // DbSets for additional entities (Redundant with earlier definitions)
        public DbSet<LibraryAPI6.Models.Member>? Member { get; set; }
        public DbSet<LibraryAPI6.Models.Employee>? Employee { get; set; }
        public DbSet<LibraryAPI6.Models.BookCopy>? BookCopy { get; set; }
        public DbSet<LibraryAPI6.Models.Admin>? Admin { get; set; }
    }
}
