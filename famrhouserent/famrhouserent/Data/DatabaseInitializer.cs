using Microsoft.EntityFrameworkCore;
using famrhouserent.Models;
using famrhouserent.Services;

namespace famrhouserent.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                context.Database.EnsureCreated();

                // Check if we already have an admin
                if (!context.UserAccounts.Any(u => u.IsAdmin))
                {
                    // Create admin user
                    var adminUser = new User
                    {
                        Email = "admin@farmhousehub.com",
                        Password = PasswordHasher.HashPassword("Admin@123"),
                        Name = "Admin",
                        IsVerified = true,
                        IsAdmin = true
                    };

                    context.UserAccounts.Add(adminUser);
                    context.SaveChanges();
                }
            }
        }
    }
}
