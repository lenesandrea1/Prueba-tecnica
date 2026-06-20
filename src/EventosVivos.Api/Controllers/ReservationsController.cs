using EventosVivos.Application.Reservations.ConfirmPayment;
using EventosVivos.Application.Reservations.Cancel;
using EventosVivos.Application.Reservations.Create;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivos.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public sealed class ReservationsController(
    ConfirmReservationPaymentCommandHandler confirmPaymentHandler,
    CancelReservationCommandHandler cancelHandler) : ControllerBase
{
    [HttpPost("{reservationId:guid}/confirm-payment")]
    [ProducesResponseType(typeof(ConfirmPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmPayment(
        Guid reservationId,
        CancellationToken cancellationToken)
    {
        var result = await confirmPaymentHandler.HandleAsync(
            new ConfirmReservationPaymentCommand(reservationId),
            cancellationToken);

        return Ok(new ConfirmPaymentResponse(
            result.ReservationId,
            result.ConfirmationCode,
            result.Status));
    }

    [HttpPost("{reservationId:guid}/cancel")]
    [ProducesResponseType(typeof(CancelReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(
        Guid reservationId,
        CancellationToken cancellationToken)
    {
        var result = await cancelHandler.HandleAsync(
            new CancelReservationCommand(reservationId),
            cancellationToken);

        return Ok(new CancelReservationResponse(
            result.ReservationId,
            result.Status,
            result.CancelledAtUtc));
    }
}

public sealed record ConfirmPaymentResponse(
    Guid ReservationId,
    string ConfirmationCode,
    string Status);

public sealed record CancelReservationResponse(
    Guid ReservationId,
    string Status,
    DateTime CancelledAtUtc);
