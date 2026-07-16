using System.ComponentModel.DataAnnotations;

namespace GameHub.API.Dtos.SaveGames;

public class UpdateSaveGameDto
{
    [Range (1, 999)]
    public int Level { get; set; }

    [Range (0, 999999)]
    public int Gold { get; set; }

    [Required]
    public string SaveDataJson { get; set; } = string.Empty;
}
