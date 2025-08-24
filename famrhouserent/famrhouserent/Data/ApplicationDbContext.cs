using Microsoft.EntityFrameworkCore;
using famrhouserent.Models;

namespace famrhouserent.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
   
        public DbSet<User> UserAccounts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Admin> tblAdmin {  get; set; }
        public DbSet<Owner> tblOwner { get; set; }
        public DbSet<FarmHouse> FarmHouses { get; set; }
        public DbSet<Booking> tblBooking { get; set; }
        public DbSet<FarmHouseImage> FarmHouseImages { get; set; }

        public DbSet<Feedback> tblFeedback { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("UserAccounts");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.IsAdmin)
                    .HasDefaultValue(false)
                    .IsRequired();
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contacts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("tblBooking");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BookingDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.BookingStatus).HasMaxLength(50).HasDefaultValue("Pending");

                entity.HasOne(e => e.UserAccount)
      .WithMany(u => u.Bookings)
      .HasForeignKey(e => e.UserId)
      .OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<FarmHouse>()
    .HasMany(f => f.Bookings)
    .WithOne(b => b.FarmHouse)
    .HasForeignKey(b => b.FarmHouseId)
    .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FarmHouseImage>(entity =>
            {
                entity.ToTable("FarmHouseImages");
                entity.HasKey(e => e.ImageId);

                entity.HasOne(e => e.FarmHouse)
                    .WithMany(f => f.FarmHouseImages)
                    .HasForeignKey(e => e.FarmHouseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Contact>()
    .HasOne(c => c.User)
    .WithMany()
    .OnDelete(DeleteBehavior.Cascade); 



        }

    }
}
