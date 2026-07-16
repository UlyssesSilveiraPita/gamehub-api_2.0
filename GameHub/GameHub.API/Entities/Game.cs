namespace GameHub.API.Entities;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;   
    public string Publisher { get; set;  } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<GameProduct> Products { get; set; } = [];

}
