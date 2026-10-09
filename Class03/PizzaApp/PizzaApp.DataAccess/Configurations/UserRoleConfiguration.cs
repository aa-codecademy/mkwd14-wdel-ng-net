using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PizzaApp.DataAccess.Configurations;

/// <summary>
/// Seeds the AspNetUserRoles row that gives the seeded admin (UserConfiguration) the Admin role.
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(new IdentityUserRole<string>
        {
            RoleId = RoleConfiguration.AdminRoleId,
            UserId = UserConfiguration.AdminUserId
        });
    }
}
