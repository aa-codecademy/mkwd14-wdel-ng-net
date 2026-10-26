using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaApp.Domain.Entities;

namespace PizzaApp.DataAccess.Context;

/// <summary>
/// The database session. IdentityDbContext adds the Identity tables (AspNetUsers, AspNetRoles, ...).
/// </summary>
public class PizzaAppDbContext : IdentityDbContext<User>
{
    public PizzaAppDbContext(DbContextOptions<PizzaAppDbContext> options) : base(options)
    {

    }

    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuring every IEntityTypeConfiguration<T> class in this project (see the Configurations folder)
        builder.ApplyConfigurationsFromAssembly(typeof(PizzaAppDbContext).Assembly);
    }

}
