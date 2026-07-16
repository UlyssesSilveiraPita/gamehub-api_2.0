using GameHub.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameHub.API.Configurations;

public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.ProductName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(item => item.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Ignore(item => item.TotalPrice);

        builder.HasOne(item => item.Purchase)
            .WithMany(purchase => purchase.Items)
            .HasForeignKey(item => item.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade); // Se uma compra ainda sem valor histórico importante for removida fisicamente, seus itens devem ser removidos junto.

        builder.HasOne(item => item.GameProduct)
            .WithMany()
            .HasForeignKey(item => item.GameProductId)
            .OnDelete(DeleteBehavior.Restrict); // apagar um produto não deve apagar itens de compras antigas.

        builder.HasIndex(item => item.PurchaseId);

        builder.HasIndex(item => item.GameProductId);
    }
}
