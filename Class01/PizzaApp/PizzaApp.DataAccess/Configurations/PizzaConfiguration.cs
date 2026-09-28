using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaApp.Domain.Entities;

namespace PizzaApp.DataAccess.Configurations;

public class PizzaConfiguration : IEntityTypeConfiguration<Pizza>
{
    public void Configure(EntityTypeBuilder<Pizza> builder)
    {
        builder.HasKey(pizza => pizza.Id);

        builder.Property(pizza => pizza.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pizza => pizza.Description)
            .HasMaxLength(500);

        // numeric(10,2): up to 99,999,999.99 with exactly 2 decimals
        builder.Property(pizza => pizza.Price)
            .HasPrecision(10, 2);

        // A pizza belongs to the user who created it; deleting the user deletes their pizzas
        builder.HasOne(pizza => pizza.User)
            .WithMany(user => user.Pizzas)
            .HasForeignKey(pizza => pizza.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
