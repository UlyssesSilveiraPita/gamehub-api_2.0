using GameHub.API.Dtos.Common;
using GameHub.API.Dtos.Purchases;
using GameHub.API.Extensions;
using GameHub.API.Common.Errors;
using GameHub.API.Mappings;
using GameHub.API.Services.Abstractions;
using GameHub.API.Validation.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameHub.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;
    private readonly ICurrentUser _currentUser;
    private readonly IValidator<CreatePurchaseRequest> _createPurchaseValidator;
    private readonly IValidator<PurchaseHistoryQuery>_purchaseHistoryQueryValidator;

    public PurchasesController(
    IPurchaseService purchaseService,
    ICurrentUser currentUser,
    IValidator<CreatePurchaseRequest> createPurchaseValidator,
    IValidator<PurchaseHistoryQuery> purchaseHistoryQueryValidator)
    {
        _purchaseService = purchaseService;
        _currentUser = currentUser;
        _createPurchaseValidator = createPurchaseValidator;
        _purchaseHistoryQueryValidator = purchaseHistoryQueryValidator;
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse),StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseResponse>> CreatePurchase(
        CreatePurchaseRequest request)
    {
        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var validation = _createPurchaseValidator.Validate(request);

        if (validation.IsInvalid)
        {
            return this.ValidationFailed(validation);
        }

        var result = await _purchaseService.CreatePurchaseAsync(
            userId,
            request.GameProductId,
            request.Quantity
        );

        if (result.IsFailure)
        {
            if (result.Error == PurchaseErrors.ProductNotFound)
            {
                return this.NotFoundError(result.Error);
            }

            return this.BadRequestError(result.Error!);
        }

        var purchase = result.Value!;

        return StatusCode(
            StatusCodes.Status201Created,
            purchase.ToResponse());


    }

    [HttpGet("{id:int}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseResponse>> GetPurchaseById(int id)
    {
        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var purchase = await _purchaseService.GetPurchaseByIdAsync(id, userId);

        if (purchase is null)
            return NotFound();

        return Ok(purchase.ToResponse());
    }

    // End Point Lista todas as compras do usuario cadastrado \\
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<PurchaseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PurchaseResponse>>> GetMyPurchases()
    {
        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var purchases = await _purchaseService.GetPurchasesByUserAsync(userId);

        var response = purchases
            .Select(purchase => purchase.ToResponse())
            .ToList();

        return Ok(response);
    }

    [HttpGet("history")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PagedResponse<PurchaseHistoryResponse>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<PurchaseHistoryResponse>>>
        GetHistory([FromQuery] PurchaseHistoryQuery query)
    {
        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var validation =
            _purchaseHistoryQueryValidator.Validate(query);

        if (validation.IsInvalid)
        {
            return this.ValidationFailed(validation);
        }

        var history = await _purchaseService
            .GetPurchaseHistoryAsync(userId, query);

        return Ok(history);
    }

}
