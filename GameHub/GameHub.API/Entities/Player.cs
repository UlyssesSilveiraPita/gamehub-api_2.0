namespace GameHub.API.Entities;

public class Player
{
    public Guid Id { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // pega a data e hora do computador do ususario no momento que ele for criado.
   
    //=============================
        // Relacionamentos \\
    //=============================
    public ICollection<SaveGame> SaveGames { get; set; } = new List<SaveGame>(); //colecoes de saves
    public ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
    public ICollection<LeaderboardEntry> LeaderboardEntries { get; set; } = new List<LeaderboardEntry>();   

}
