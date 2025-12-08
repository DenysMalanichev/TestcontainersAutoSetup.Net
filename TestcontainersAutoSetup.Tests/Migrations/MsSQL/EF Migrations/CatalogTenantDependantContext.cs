using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Data;

public class CatalogTenantDependantContext : DbContext
{
#pragma warning disable CS8618 // Required by Entity Framework

    // ITenant is a dummy dependency to test the DI context instantiation
    public CatalogTenantDependantContext(
        DbContextOptions<CatalogTenantDependantContext> options,
        ITenantProvider tenant)
            : base(options) { }

    public DbSet<Basket> Baskets { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Seed();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}