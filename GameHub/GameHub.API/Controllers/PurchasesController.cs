using GameHub.API.Dtos.Purchases;
using GameHub.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameHub.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly PurchaseService _purchaseService;

    public PurchasesController(PurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseResponse>> CreatePurchase(
    CreatePurchaseRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var purchase = await _purchaseService.CreatePurchaseAsync(
            userId,
            request.GameProductId,
            request.Quantity
        );

        var response = new PurchaseResponse
        {
            Id = purchase.Id,
            Status = purchase.Status.ToString(),
            TotalAmount = purchase.TotalAmount,
            Currency = purchase.Currency,
            CreatedAt = purchase.CreatedAt,
            Items = purchase.Items
                .Select(item => new PurchaseItemResponse
                {
                    GameProductId = item.GameProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                })
                .ToList()
        };

        return StatusCode(StatusCodes.Status201Created, response);


    }

    [HttpGet("{id:int}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseResponse>> GetPurchaseById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var purchase = await _purchaseService.GetPurchaseByIdAsync(id, userId);

        if (purchase is null)
            return NotFound();

        var response = new PurchaseResponse
        {
            Id = purchase.Id,
            Status = purchase.Status.ToString(),
            TotalAmount = purchase.TotalAmount,
            Currency = purchase.Currency,
            CreatedAt = purchase.CreatedAt,
            Items = purchase.Items
                .Select(item => new PurchaseItemResponse
                {
                    GameProductId = item.GameProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                })
                .ToList()
        };

        return Ok(response);
    }

    // End Point Lista todas as compras do usuario cadastrado \\
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<PurchaseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PurchaseResponse>>> GetMyPurchases()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var purchases = await _purchaseService.GetPurchasesByUserAsync(userId);

        var response = purchases.Select(p => new PurchaseResponse
        {
            Id = p.Id,
            Status = p.Status.ToString(),
            TotalAmount = p.TotalAmount,
            Currency = p.Currency,
            CreatedAt = p.CreatedAt,
            Items = p.Items.Select(item => new PurchaseItemResponse
            {
                GameProductId = item.GameProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice
            }).ToList()
        }).ToList();

        return Ok(response);
    }



}
