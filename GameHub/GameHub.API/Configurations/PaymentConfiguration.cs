using GameHub.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameHub.API.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(payment => payment.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(payment => payment.Status)
            .IsRequired();

        builder.Property(payment => payment.PaymentMethod)
            .IsRequired();

        builder.Property(payment => payment.IdempotencyKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(payment => payment.ExternalTransactionId)
            .HasMaxLength(150);

        builder.Property(payment => payment.CreatedAt)
            .IsRequired();

        builder.HasOne(payment => payment.Purchase)
            .WithMany(purchase => purchase.Payments)
            .HasForeignKey(payment => payment.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(payment => payment.PurchaseId);

        builder.HasIndex(payment => payment.Status);

        builder.HasIndex(payment => payment.IdempotencyKey)
            .IsUnique();
    }
}
