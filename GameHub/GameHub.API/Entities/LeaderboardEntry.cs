namespace GameHub.API.Entities;

public class LeaderboardEntry
{
    public Guid Id { get; set; } // identificador de entrada ao ranking 
    public Guid PlayerId { get; set; } // quem fes a pontuacao
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; } // data do registro da pontuacao

    //=============================
    // Navigation  Property \\
    //=============================

    public Player Player { get; set; } = null!;
}
