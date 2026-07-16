using System.ComponentModel.DataAnnotations;

namespace GameHub.API.Dtos.Achievements;

public class CreateAchievementDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    [Range(1, 1000)]
    public int Points { get; set; } 
}
