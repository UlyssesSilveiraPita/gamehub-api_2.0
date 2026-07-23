using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GameHub.API.Dtos.Auth;
using GameHub.API.Entities;
using GameHub.API.Dtos.Common;
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
    [ProducesResponseType<LoginResponseDto>(
       StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(
       StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(
       LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(
            dto.Username);

        if (user is null)
        {
            return Unauthorized(
                new ApiErrorResponse
                {
                    Code = "auth.invalid_credentials",
                    Message = "Invalid username or password."
                });
        }

        var signInResult =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                false);

        if (!signInResult.Succeeded)
        {
            return Unauthorized(
                new ApiErrorResponse
                {
                    Code = "auth.invalid_credentials",
                    Message = "Invalid username or password."
                });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var expiresAt = DateTime.UtcNow.AddHours(2);

        var claims = new List<Claim>
    {
        new(
            ClaimTypes.NameIdentifier,
            user.Id),

        new(
            ClaimTypes.Name,
            user.UserName!)
    };

        claims.AddRange(
            roles.Select(
                role => new Claim(
                    ClaimTypes.Role,
                    role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString =
            new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(
            new LoginResponseDto
            {
                Token = tokenString,
                ExpiresAt = expiresAt,
                User = new AuthenticatedUserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email,
                    Roles = roles.ToArray()
                }
            });
    }

    [Authorize]
    [HttpGet("Me")]
    [ProducesResponseType<AuthenticatedUserResponseDto>(
    StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticatedUserResponseDto>> Me()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new ApiErrorResponse
                {
                    Code = "auth.invalid_session",
                    Message = "The authenticated session is invalid."
                });
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Unauthorized(
                new ApiErrorResponse
                {
                    Code = "auth.invalid_session",
                    Message = "The authenticated session is invalid."
                });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(
            new AuthenticatedUserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email,
                Roles = roles.ToArray()
            });
    }

}
