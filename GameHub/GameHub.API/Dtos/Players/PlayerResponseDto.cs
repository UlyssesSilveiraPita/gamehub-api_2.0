
namespace GameHub.API.Dtos.Players;

public class PlayerResponseDto
{
    public Guid Id { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Experience { get; set; }
    public DateTime CreatedAt { get; set; }
}
