using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaApp.Domain.Entities;

namespace PizzaApp.DataAccess.Configurations;

/// <summary>
/// Seeds the first Admin account into the AspNetUsers table through a migration: "admin" / "Admin123!".
/// Every value is hardcoded, because HasData needs FIXED values (see RoleConfiguration).
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    // Public, because UserRoleConfiguration gives this user the Admin role
    public const string AdminUserId = "860135b1-1222-4cd1-9dff-c04fb8c94843";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Seed Admin user
        builder.HasData(new User
        {
            Id = AdminUserId,

            // Identity finds users by the normalized (upper-case) name and email
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@pizzaapp.local",
            NormalizedEmail = "ADMIN@PIZZAAPP.LOCAL",

            // Admin123!
            PasswordHash = "AQAAAAIAAYagAAAAEORMNJTSPwqrmb2GKtm5oxli8mkCddIShGuomTmTzpYUfNBuyB9YyZyhymbYQOXTMA==",

            SecurityStamp = "ea93155b-08c2-4539-a6d5-e922d27173e3",
            ConcurrencyStamp = "d476b4e7-13dc-42c4-a6a2-0c1c5e8897be"
        });
    }
}
