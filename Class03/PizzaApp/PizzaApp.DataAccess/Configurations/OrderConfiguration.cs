using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaApp.Domain.Entities;

namespace PizzaApp.DataAccess.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);

        builder.Property(order => order.AddressTo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(order => order.Description)
            .HasMaxLength(500);

        // Stored as text ("Pending", "Delivered", ...) so the table is readable in pgAdmin
        builder.Property(order => order.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Relations

        // An order belongs to the user who placed it; deleting the user deletes their orders
        builder.HasOne(order => order.User)
            .WithMany(user => user.Orders)
            .HasForeignKey(order => order.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // The pizzas of an order are deleted together with the order
        builder.HasMany(order => order.Pizzas)
            .WithOne(pizza => pizza.Order)
            .HasForeignKey(pizza => pizza.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
