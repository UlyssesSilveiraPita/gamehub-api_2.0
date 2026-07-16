namespace GameHub.API.Dtos.PlayerAchievements;

public class PlayerAchievementResponseDto
{
    public Guid PlayerId { get; set; }
    public Guid AchievementId { get; set; }
    public DateTime UnlockedAt { get; set; }

}
