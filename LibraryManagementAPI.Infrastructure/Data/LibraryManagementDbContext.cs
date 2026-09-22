using LibraryManagementAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Infrastructure.Data
{
    public class LibraryManagementDbContext : DbContext
    {
        public LibraryManagementDbContext(
            DbContextOptions<LibraryManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<MembershipPlan> MembershipPlans { get; set; }

        public DbSet<UserMembership> UserMemberships { get; set; }

        public DbSet<MembershipPayment> MembershipPayments { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Loan> Loans { get; set; }

        public DbSet<Fine> Fines { get; set; }

        public DbSet<FinePayment> FinePayments { get; set; }

        public DbSet<Library> Libraries { get; set; }

        public DbSet<LibraryBook> LibraryBooks { get; set; }

        public DbSet<UserLibrary> UserLibraries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MembershipPlan>()
.Property(x => x.Price)
.HasPrecision(18, 2);

            modelBuilder.Entity<MembershipPayment>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Fine>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FinePayment>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
                .HasMany(x => x.UserMemberships)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<MembershipPlan>()
                .HasMany(x => x.UserMemberships)
                .WithOne(x => x.MembershipPlan)
                .HasForeignKey(x => x.MembershipPlanId);

            modelBuilder.Entity<UserMembership>()
                .HasMany(x => x.MembershipPayments)
                .WithOne(x => x.UserMembership)
                .HasForeignKey(x => x.UserMembershipId);

            modelBuilder.Entity<Author>()
                .HasMany(x => x.Books)
                .WithOne(x => x.Author)
                .HasForeignKey(x => x.AuthorId);

            modelBuilder.Entity<Category>()
                .HasMany(x => x.Books)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Loans)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<LibraryBook>()
                .HasMany(x => x.Loans)
                .WithOne(x => x.LibraryBook)
                .HasForeignKey(x => x.LibraryBookId);

            modelBuilder.Entity<Loan>()
                .HasMany(x => x.Fines)
                .WithOne(x => x.Loan)
                .HasForeignKey(x => x.LoanId);

            modelBuilder.Entity<Fine>()
                .HasMany(x => x.FinePayments)
                .WithOne(x => x.Fine)
                .HasForeignKey(x => x.FineId);

            modelBuilder.Entity<Library>()
                .HasMany(x => x.LibraryBooks)
                .WithOne(x => x.Library)
                .HasForeignKey(x => x.LibraryId);

            modelBuilder.Entity<Book>()
                .HasMany(x => x.LibraryBooks)
                .WithOne(x => x.Book)
                .HasForeignKey(x => x.BookId);

            modelBuilder.Entity<User>()
    .HasMany(x => x.UserLibraries)
    .WithOne(x => x.User)
    .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Library>()
                .HasMany(x => x.UserLibraries)
                .WithOne(x => x.Library)
                .HasForeignKey(x => x.LibraryId);

        }
    }
}