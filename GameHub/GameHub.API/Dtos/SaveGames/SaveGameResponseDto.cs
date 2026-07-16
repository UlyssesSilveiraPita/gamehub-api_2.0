namespace GameHub.API.Dtos.SaveGames;

public class SaveGameResponseDto
{
    public Guid Id { get; set; } // Id do save
    public Guid PlayerId { get; set; } // Id do personagem
    public int Level { get; set; }
    public int Gold { get; set; }
    public string SaveDataJson { get; set; } = string.Empty; 
    public DateTime LastSavedAt { get; set; }



}
