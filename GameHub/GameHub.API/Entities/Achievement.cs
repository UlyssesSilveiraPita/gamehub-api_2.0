using System.ComponentModel.DataAnnotations;

namespace GameHub.API.Entities;


public class Achievement
{
    public Guid Id { get; set; } 
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty; // recebe vasio
    [Range(1, 1000)]
    public int Points { get; set; }

    //=============================
    // Relacionamentos \\
    //=============================

    public ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>(); //descobre quem desbloqueou a conquista
}
