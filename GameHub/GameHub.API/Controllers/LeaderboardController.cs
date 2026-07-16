using GameHub.API.Data;
using GameHub.API.Dtos.Leaderboard;
using GameHub.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GameHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly GameHubDbContext _context;

    public LeaderboardController(GameHubDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LeaderboardEntryResponseDto>> Create(CreateLeaderboardEntryDto dto)
    {
        var player = await _context.Players.FindAsync(dto.PlayerId);

        if (player is null || player.IsDeleted)
        {
            return BadRequest("Player nao encontrado");
        }

        var entry = new LeaderboardEntry
        {
            Id = Guid.NewGuid(),
            PlayerId = dto.PlayerId,
            Score = dto.Score,
            CreatedAt = DateTime.UtcNow
        };

        _context.LeaderboardEntries.Add(entry); // add 

        await _context.SaveChangesAsync();

        var response = new LeaderboardEntryResponseDto
        {
            Id = entry.Id,
            PlayerId = player.Id,
            PlayerName = player.NickName,
            Score = entry.Score,
            CreatedAt = entry.CreatedAt
        };

        return Ok(response);
    }

    [HttpGet] // lista os melhores jogadores
    public async Task<ActionResult<IEnumerable<LeaderboardEntryResponseDto>>> GetAll()
    {
        var leaderboard = await _context.LeaderboardEntries
            .OrderByDescending(l => l.Score)
            .Select(l => new LeaderboardEntryResponseDto
            {
                Id = l.Id,
                PlayerId = l.PlayerId,
                PlayerName = l.Player.NickName,
                Score = l.Score,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return Ok(leaderboard);
    }

    [HttpGet("Top10")] // lista os  10 melhores jogadores
    public async Task<ActionResult<IEnumerable<LeaderboardEntryResponseDto>>> GetTop10()
    {
        var leaderboard = await _context.LeaderboardEntries
            .OrderByDescending(l => l.Score)
            .Take(10) // vai fazer uma lista com os 10 primeiros do rank
            .Select(l => new LeaderboardEntryResponseDto
            {
                Id = l.Id,
                PlayerId = l.PlayerId,
                PlayerName = l.Player.NickName,
                Score = l.Score,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return Ok(leaderboard);

    }

    [HttpGet("Player/{playerId}")] //mostra toda a pontuacao de um jogador especifico
    public async Task<ActionResult<IEnumerable<LeaderboardEntryResponseDto>>> GetByPlayer(Guid playerId)
    {
        var playerExists = await _context.Players.AnyAsync(p => p.Id == playerId && !p.IsDeleted);

        if(!playerExists)
        {
            return NotFound("Player nao encontrado");
        }

        var entries = await _context.LeaderboardEntries
            .Where(l => l.PlayerId == playerId)
            .OrderByDescending(l => l.Score)
            .Select(l => new LeaderboardEntryResponseDto
            {
                Id = l.Id,
                PlayerId = l.PlayerId,
                PlayerName = l.Player.NickName,
                Score = l.Score,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return Ok(entries);
    }

}
