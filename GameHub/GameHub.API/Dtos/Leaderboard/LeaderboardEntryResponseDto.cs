using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameHub.API.Dtos.Leaderboard;

public class LeaderboardEntryResponseDto
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }


}
