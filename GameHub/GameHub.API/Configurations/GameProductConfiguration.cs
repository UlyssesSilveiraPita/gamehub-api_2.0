using GameHub.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.InteropServices;

namespace GameHub.API.Configurations;

public class GameProductConfiguration : IEntityTypeConfiguration<GameProduct>
{
    public void Configure(EntityTypeBuilder<GameProduct> builder)
    {
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(product => product.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(product => product.ProductType)
            .IsRequired();

        builder.Property(product => product.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(product => product.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(product => product.IsActive)
            .HasDefaultValue(true);

        builder.Property(product => product.CreatedAt)
            .IsRequired();

        builder.HasOne(product => product.Game)
            .WithMany(game => game.Products)
            .HasForeignKey(product => product.GameId)
            .OnDelete(DeleteBehavior.Restrict); // impede que um jogo seja excluído fisicamente enquanto ainda possuir produtos ligados a ele.

        builder.HasIndex(product => product.GameId);

        builder.HasIndex(product => product.Name);

    }

}
