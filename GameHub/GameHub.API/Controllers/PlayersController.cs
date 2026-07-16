using System.Security.Claims;
using GameHub.API.Data;
using GameHub.API.Entities;
using GameHub.API.Dtos.Players;
using GameHub.API.Dtos.SaveGames;
using GameHub.API.Dtos.PlayerAchievements;
using GameHub.API.Dtos.Achievements;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace GameHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly GameHubDbContext _context; // acessa o banco de dados

    public PlayersController(GameHubDbContext context)
    {
        _context = context;
    }

    [HttpGet] // pega todos os player do banco de dados
    public async Task<ActionResult<IEnumerable<PlayerResponseDto>>> Getall()
    {
        var players = await _context.Players
            .Where(p => !p.IsDeleted)
            .Select(p => new PlayerResponseDto
            {
                Id = p.Id,
                NickName = p.NickName,
                Level = p.Level,
                Experience = p.Experience,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(players);

    }

    [Authorize]
    [HttpPost] // Criacao do Player 
    public async Task<ActionResult<PlayerResponseDto>> Create(CreatePlayerDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized("Usuário não autenticado.");
        }

        var player = new Player
        {
            Id = Guid.NewGuid(),
            NickName = dto.NickName,
            Level = 1,
            Experience = 0,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var response = new PlayerResponseDto
        {
            Id = player.Id,
            NickName = player.NickName,
            Level = player.Level,
            Experience = player.Experience,
            CreatedAt = DateTime.UtcNow,
        };


        return CreatedAtAction(nameof(GetById), new { id = player.Id }, response);
    }

    [HttpGet("{id}")] // pega player pelo Id
    public async Task<ActionResult<PlayerResponseDto>> GetById(Guid id)
    {
        var player = await _context.Players.FindAsync(id);

        if (player is null || player.IsDeleted)
        {
            return NotFound();
        }

        var response = new PlayerResponseDto
        {
            Id = player.Id,
            NickName = player.NickName,
            Level = player.Level,
            Experience = player.Experience,
            CreatedAt = player.CreatedAt

        };

        return Ok(response);
    }

    [HttpPut("{id}")] // atualizad o jogador
    public async Task<ActionResult> Update(Guid id, UpdatePlayerDto dto)
    {
        var player = await _context.Players.FindAsync(id);

        if (player is null || player.IsDeleted)
        {
            return NotFound();
        }

        player.NickName = dto.NickName;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")] //marca que foi deletado mas permanece no banco de dados
    public async Task<ActionResult> Delete(Guid id)
    {
        var player = await _context.Players.FindAsync(id);

        if (player is null || player.IsDeleted)
        {
            return NoContent();
        }

        player.IsDeleted = true;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{playerId}/SaveGames")]
    public async Task<ActionResult<IEnumerable<SaveGameResponseDto>>> GetPlayerSaveGames(Guid playerId)
    {
        var playerExists = await _context.Players
           .AnyAsync(p => p.Id == playerId && !p.IsDeleted); // verificacao se a sabes no bando de dados

        if (!playerExists)
        {
            return NotFound("Player não Encontrado");
        }

        var saveGames = await _context.SaveGames
            .Where(s => s.PlayerId == playerId) // filto para saves especificos de cada jogador
            .Select(s => new SaveGameResponseDto
            {
                Id = s.Id,
                PlayerId = s.PlayerId,
                Level = s.Level,
                Gold = s.Gold,
                SaveDataJson = s.SaveDataJson,
                LastSavedAt = s.LastSavedAt
            })
            .ToListAsync();

        return Ok(saveGames);
    }

    [HttpPost("{playerId}/Achievements/{achievementId}")]
    public async Task<ActionResult<PlayerAchievementResponseDto>> UnlockAchievement(
        Guid playerId,
        Guid achievementId)
    {
        var playerExists = await _context.Players // valida se o player existe
            .AnyAsync(p => p.Id == playerId && !p.IsDeleted);

        if (!playerExists)
        {
            return NotFound("Player não encontrado");
        }

        var achievementExists = await _context.Achievements
            .AnyAsync(a => a.Id == achievementId);

        if (!achievementExists) // valida se o achievement existe
        {
            return NotFound("Achievement não encontrado.");
        }

        var alreadyUnloacked = await _context.PlayerAchievements
            .AnyAsync(pa => pa.PlayerId == playerId && pa.AchievementId == achievementId);

        if (alreadyUnloacked) // valida se o player ja liberou o achievement
        {
            return Conflict("Esse player ja desbloqueou esse Achievement.");
        }

        var playerAchievement = new PlayerAchievement
        {
            PlayerId = playerId,
            AchievementId = achievementId,
            UnlockedAt = DateTime.UtcNow
        };

        _context.PlayerAchievements.Add(playerAchievement);
        await _context.SaveChangesAsync();

        var response = new PlayerAchievementResponseDto
        {
            PlayerId = playerId,
            AchievementId = playerAchievement.AchievementId,
            UnlockedAt = playerAchievement.UnlockedAt
        };


        return Ok(response);
    }

    [HttpGet("{playerId}/Achievements")]
    public async Task<ActionResult<IEnumerable<AchievementResponseDto>>> GetPlayerAchievements(Guid playerId)
    {
        var playerExists = await _context.Players
            .AnyAsync(p => p.Id == playerId && !p.IsDeleted);

        if (!playerExists)
        {
            return NotFound("Player não encontrado");
        }

        var achievements = await _context.PlayerAchievements
            .Where(pa => pa.PlayerId == playerId)
            .Select(pa => new AchievementResponseDto
            {
                Id = pa.Achievement.Id,
                Name = pa.Achievement.Name,
                Description = pa.Achievement.Description,
                Points = pa.Achievement.Points
            })
            .ToListAsync();

        return Ok(achievements);
    }

    [Authorize] // exige token
    [HttpPost("{playerId}/LinkMe")]
    public async Task<ActionResult> LinkMe(Guid playerId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // pego o Id do Usuario de dentro do JWT

        if(string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Usuario nao autenticado");
        }

        var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == playerId);


        if (player is null || player.IsDeleted)
        {
            return NotFound("Player nao encontrado");
        }

        //var linkedPlayers = await _context.Players
        //    .Where(p => p.UserId == userId)
        //    .ToListAsync();

        //foreach (var linkedPlayer in linkedPlayers)
        //{
        //    linkedPlayer.UserId = null;
        //}

        player.UserId = userId;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Player vinculado ao usuário autenticado com sucesso.",
            PlayerId = player.Id, // busca o player pelo Id
            player.NickName,
            UserId = userId // salva o Id dentro do Player
        });

    }

    [Authorize]
    [HttpGet("MyPlayers")]
    public async Task<ActionResult<IEnumerable<PlayerResponseDto>>> GetMyPlayers()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized("Usuário não autenticado.");
        }

        var players = await _context.Players
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .Select(p => new PlayerResponseDto
            {
                Id = p.Id,
                NickName = p.NickName,
                Level = p.Level,
                Experience = p.Experience,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(players);
    }
















}
