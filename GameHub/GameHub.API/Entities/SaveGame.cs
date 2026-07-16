namespace GameHub.API.Entities;

public class SaveGame
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public int Level { get; set; }
    public int Gold { get; set; }
    public string SaveDataJson { get; set; } = string.Empty; // JSON Inteiro
    public DateTime LastSavedAt { get; set; }
    public Player Player { get; set; } = null!;
}
