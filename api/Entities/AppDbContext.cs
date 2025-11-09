using Microsoft.EntityFrameworkCore;

namespace UserNotepad.Entities
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserAttribute> Attributes { get; set; }
        public DbSet<Operator> Operators { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(x => x.ID);
            modelBuilder.Entity<User>()
                .Property(x => x.Name)
                .HasMaxLength(50);
            modelBuilder.Entity<User>()
                .Property(x => x.Surname)
                .HasMaxLength(150);

            modelBuilder.Entity<UserAttribute>()
                .HasKey(x => x.ID);
            modelBuilder.Entity<UserAttribute>()
                .HasOne(x => x.User)
                .WithMany(x => x.Attributes)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Operator>()
                .HasKey(x => x.ID);
        }
    }
}
