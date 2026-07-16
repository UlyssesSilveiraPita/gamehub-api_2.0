namespace GameHub.API.Entities;

// Importante vai conectar Player => PlayerAchivement => Achievement
public class PlayerAchievement 
{
    public Guid PlayerId { get; set; }
    public Guid AchievementId { get; set; }
    public DateTime UnlockedAt { get; set; }

    //=============================
    // Navigation Properties \\
    //=============================

    public Player Player { get; set; } = null!; // EF Core vai preencher depois
    public Achievement Achievement { get; set; } = null!; // EF Core vai preencher depois


}
