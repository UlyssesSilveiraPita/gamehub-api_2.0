using System.Security.Claims;
using GameHub.API.Dtos.Payments;
using GameHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameHub.API.Controllers;

[Authorize]
[ApiController]
[Route("api/purchases/{purchaseId:int}/payments")] // uma compra -> seus pagamentos
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponse>> CreatePayment(
    int purchaseId,
    CreatePaymentRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        try
        {
            var payment = await _paymentService.CreatePaymentAsync(
                purchaseId,
                userId,
                request.PaymentMethod);

            var response = new PaymentResponse
            {
                Id = payment.Id,
                PurchaseId = payment.PurchaseId,
                Status = payment.Status.ToString(),
                PaymentMethod = payment.PaymentMethod.ToString(),
                Amount = payment.Amount,
                Currency = payment.Currency,
                IdempotencyKey = payment.IdempotencyKey,
                ExternalTransactionId = payment.ExternalTransactionId,
                CreatedAt = payment.CreatedAt
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPost("/api/payments/{paymentId:int}/approve")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponse>> ApprovePayment(
    int paymentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        try
        {
            var payment = await _paymentService.ApprovePaymentAsync(
                paymentId,
                userId);

            var response = new PaymentResponse
            {
                Id = payment.Id,
                PurchaseId = payment.PurchaseId,
                Status = payment.Status.ToString(),
                PaymentMethod = payment.PaymentMethod.ToString(),
                Amount = payment.Amount,
                Currency = payment.Currency,
                IdempotencyKey = payment.IdempotencyKey,
                ExternalTransactionId = payment.ExternalTransactionId,
                CreatedAt = payment.CreatedAt
            };

            return Ok(response);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }



}
