using GameHub.API.Entities;
using GameHub.API.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameHub.API.Data.Seed;

public class DatabaseSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly GameHubDbContext _context;

    public DatabaseSeeder(
        UserManager<ApplicationUser> userManager,
        GameHubDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task SeedAsync()
    {
        const string adminEmail = "admin@gamehub.com";
        const string adminPassword = "GameHub@123";

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        //criando o usuario\\

        if (adminUser is null) // procura por usuario caso nao tenha cria...
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));

                throw new Exception($"Error creating admin user: {errors}");
            }
        }

        //criando um novo produto\\

        var game = await _context.Games.
            FirstOrDefaultAsync(game => game.Title == "Wild Hunter");

        if(game is null)
        {
            game = new Game
            {
                Title = "Wild Hunter",
                Description = "An Adventure RPG built with Unity.",
                Developer = "Calisto Interactive",
                Publisher = "Calisto Interactive",
                ReleaseDate = new DateTime(2026, 1, 1),
                CoverImageUrl = "/images/Wild-Hunter.png",
                IsActive = true
            };

            _context.Games.Add(game);
            
            await _context.SaveChangesAsync();
        }

        var baseGame = await _context.GameProducts
            .FirstOrDefaultAsync(product =>
            product.GameId == game.Id &&
            product.ProductType == GameProductType.BaseGame);

        if (baseGame is null)
        {
            baseGame = new GameProduct
            {
                GameId = game.Id,
                Name = "Wild Hunter - Base Game",
                Description = "Base edition of Wild Hunter.",
                ProductType = GameProductType.BaseGame,
                Price = 59.90m,
                Currency = "BRL",
                IsActive = true
            };

            _context.GameProducts.Add(baseGame);

            await _context.SaveChangesAsync();
        }

    }
        
}


