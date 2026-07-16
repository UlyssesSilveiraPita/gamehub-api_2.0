using System.Security.Claims;
using GameHub.API.Data;
using GameHub.API.Dtos.SaveGames;
using GameHub.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace GameHub.API.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class SaveGamesController : Controller
{
    private readonly GameHubDbContext _context;

    public SaveGamesController(GameHubDbContext context)
    {
        _context = context;
    }

    [HttpGet] //Pega todos os saves
    public async Task<ActionResult<IEnumerable<SaveGameResponseDto>>> GetAll()
    {
        var saveGames = await _context.SaveGames
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

    [HttpPost]
    public async Task<ActionResult<SaveGameResponseDto>> Create(CreateSaveGameDto dto)
    {
        var player = await _context.Players.FindAsync(dto.PlayerId); // pegando o player

        if (player is null || player.IsDeleted)
        {
            return BadRequest("Player não encontrado");
        }

        var saveGame = new SaveGame
        {
            Id = Guid.NewGuid(),
            PlayerId = dto.PlayerId,
            Level = dto.Level,
            Gold = dto.Gold,
            SaveDataJson = dto.SaveDataJson,
            LastSavedAt = DateTime.UtcNow // tempo atual na hora do save
        };

        _context.SaveGames.Add(saveGame);
        player.Level = dto.Level;
        await _context.SaveChangesAsync();

        var response = new SaveGameResponseDto
        {
            Id = saveGame.Id,
            PlayerId = saveGame.PlayerId,
            Level = saveGame.Level,
            Gold = saveGame.Gold,
            SaveDataJson = saveGame.SaveDataJson,
            LastSavedAt = saveGame.LastSavedAt
        };



        return CreatedAtAction(nameof(GetById), new { id = saveGame.Id }, response);
    }

    [HttpGet("{id}")] // pega o save pelo id
    public async Task<ActionResult<SaveGameResponseDto>> GetById(Guid id)
    {
        var saveGame = await _context.SaveGames.FindAsync(id);

        if (saveGame is null)
        {
            return NotFound();
        }

        var response = new SaveGameResponseDto
        {
            Id = saveGame.Id,
            PlayerId = saveGame.PlayerId,
            Level = saveGame.Level,
            Gold = saveGame.Gold,
            SaveDataJson = saveGame.SaveDataJson,
            LastSavedAt = saveGame.LastSavedAt
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateSaveGameDto dto)
    {
        var saveGame = await _context.SaveGames.FindAsync(id);

        if(saveGame is null)
        {
            return NotFound();
        }

        saveGame.Level = dto.Level;
        saveGame.Gold = dto.Gold;
        saveGame.SaveDataJson = dto.SaveDataJson;
        saveGame.LastSavedAt = DateTime.UtcNow;

        var player = await _context.Players.FindAsync(saveGame.PlayerId);

        if (player is not null)
        {
            player.Level = dto.Level;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize] // Retorna os saves do usuario
    [HttpGet("MySaves")]
    public async Task<ActionResult<IEnumerable<SaveGameResponseDto>>> GetMySaves()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier) ?.Value;

        if(string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Usuario nao Autenticado");
        }

        var saves = await _context.SaveGames
            .Where(s => s.Player.UserId == userId)
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

        return Ok(saves);



    }

}
