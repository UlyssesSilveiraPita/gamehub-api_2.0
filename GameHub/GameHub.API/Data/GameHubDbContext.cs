using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GameHub.API.Entities;

namespace GameHub.API.Data;

public class GameHubDbContext : IdentityDbContext<ApplicationUser>
{
    public GameHubDbContext(DbContextOptions<GameHubDbContext> options) : base(options)
    {

    }

    //=============================
    // criacao das tabelas \\
    //=============================

    public DbSet<Player> Players { get; set; } 
    public DbSet<Game> Games { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<PlayerAchievement> PlayerAchievements { get; set; }
    public DbSet<SaveGame> SaveGames { get; set; }
    public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(GameHubDbContext).Assembly); // procura dentro do projeto todas as classe que implementam IEntityTypeConfiguration<T>

        modelBuilder.Entity<PlayerAchievement>()
            .HasKey(pa => new { pa.PlayerId, pa.AchievementId });

        modelBuilder.Entity<PlayerAchievement>()
            .HasOne(pa => pa.Player)
            .WithMany(p => p.PlayerAchievements)
            .HasForeignKey(pa => pa.PlayerId);

        modelBuilder.Entity<PlayerAchievement>()
            .HasOne(pa => pa.Achievement)
            .WithMany(a => a.PlayerAchievements)
            .HasForeignKey(pa => pa.AchievementId);

        modelBuilder.Entity<Player>()
            .HasOne(p => p.User) // player tem um usuario
            .WithMany() // Um usuário pode possuir vários players.
            .HasForeignKey("UserId") //FK fica a tabela players
            .OnDelete(DeleteBehavior.SetNull); // se usuario for apagado, o player nao e apagado so fica sem usuario
    }



}
