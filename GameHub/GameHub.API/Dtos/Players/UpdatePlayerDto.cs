using System.ComponentModel.DataAnnotations;

namespace GameHub.API.Dtos.Players;

public class UpdatePlayerDto
{
    [Required]
    [MaxLength(50)]
    public string NickName { get; set; } = string.Empty;    
}
