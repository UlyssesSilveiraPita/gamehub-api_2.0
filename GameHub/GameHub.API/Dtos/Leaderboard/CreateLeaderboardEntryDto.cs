using System.ComponentModel.DataAnnotations;

namespace GameHub.API.Dtos.Leaderboard;

public class CreateLeaderboardEntryDto
{
    [Required]
    public Guid PlayerId { get; set; }
    
    [Range (0, int.MaxValue)]
    public int Score { get; set; }

}
