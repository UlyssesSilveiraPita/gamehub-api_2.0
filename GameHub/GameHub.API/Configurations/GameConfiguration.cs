using GameHub.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameHub.API.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game> //Contem as regras de persistencia da entidade Game
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(game => game.Id);
        
        builder.Property(game => game.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(game => game.Description)
            .IsRequired() 
            .HasMaxLength(1000);

        builder.Property(game => game.Developer)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(game => game.Publisher)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(game => game.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(game => game.ReleaseDate)
            .IsRequired();

        builder.Property(game => game.IsActive)
            .HasDefaultValue(true);

        builder.Property(game => game.CreatedAt)
            .IsRequired();

        builder.HasIndex(game => game.Title); // ajuda a buscar por titulo

    }

}
