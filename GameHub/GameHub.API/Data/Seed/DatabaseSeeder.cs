using GameHub.API.Entities;
using GameHub.API.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameHub.API.Data.Seed;

public class DatabaseSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly GameHubDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DatabaseSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        GameHubDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task SeedAsync()
    {
        const string adminEmail = "admin@gamehub.com";
        const string adminPassword = "GameHub@123";

        const string adminRole = "Admin";

        if (!await _roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await _roleManager.CreateAsync(
                new IdentityRole(adminRole));

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Error creating admin role: {errors}");
            }
        }

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

                    throw new InvalidOperationException(
                        $"Error creating admin user: {errors}");
            }
        }

        if (!await _userManager.IsInRoleAsync(
            adminUser,
            adminRole))
        {
            var roleAssignmentResult =
                await _userManager.AddToRoleAsync(
                    adminUser,
                    adminRole);

            if (!roleAssignmentResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleAssignmentResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Error assigning admin role: {errors}");
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


