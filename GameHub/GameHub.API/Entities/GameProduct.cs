using GameHub.API.Entities.Enums;

namespace GameHub.API.Entities;

public class GameProduct
{
    public int Id { get; set; } 

    public int GameId { get; set; } // chave estrangeira

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public GameProductType ProductType { get; set; }

    public decimal Price { get; set; }

    public string Currency { get; set; } = "BRL";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Game Game { get; set; } = null!; // Propriedade de Navegacao Permite o EF Core entender.
}
