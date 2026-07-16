using GameHub.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameHub.API.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(purchase => purchase.Id);

        builder.Property(purchase => purchase.UserId)
            .IsRequired();

        builder.Property(purchase => purchase.Status)
            .IsRequired();

        builder.Property(purchase => purchase.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(purchase => purchase.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(purchase => purchase.CreatedAt)
            .IsRequired();

        builder.HasOne(purchase => purchase.User)
            .WithMany()
            .HasForeignKey(purchase => purchase.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(purchase => purchase.UserId);

        builder.HasIndex(purchase => purchase.Status);
    }
}
