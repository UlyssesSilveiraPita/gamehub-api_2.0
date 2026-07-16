using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GameHub.API.Dtos.Auth;
using GameHub.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GameHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    //Construtor que cuida do acesso
    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("Register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        var userExists = await _userManager.FindByNameAsync(dto.UserName);

        if (userExists != null)
        {
            return BadRequest("Usuario ja Existe");
        }

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, dto.Password); // cria usuario novo

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok("Usuario criado com Sucesso");

    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username); // busca usuario

        if (user is null)
        {
            return Unauthorized("Usuario ou senha Invalidos");
        }

        //compara senhas digitada com banco de dados
        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            false);

        if(!result.Succeeded)
        {
            return Unauthorized("Usuario ou se");
        }

        //return Ok("Login realizado com sucesso");

        //===================================
        // Bloco para retorno de token JWT \\
        //===================================

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString
        });

    }
    
    [Authorize]
    [HttpGet("Me")]
    public ActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            UserName = User.Identity?.Name
        });
    }

}
