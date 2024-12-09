using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySpot.Application.Abstractions;
using MySpot.Application.Commands;

namespace MySpot.Api.Controllers
{
    [ApiController]
    [Route("parking-spots")]
    public class ReservationsController : ControllerBase
    {
        private readonly ICommandHandler<ReserveParkingSpotForVehicle> _reserveParkingSpotsForVehicleHandler;
        private readonly ICommandHandler<ReserveParkingSpotForCleaning> _reserveParkingSpotsForCleaningHandler;
        private readonly ICommandHandler<ChangeReservationLicensePlate> _changeReservationLicencePlateHandler;
        private readonly ICommandHandler<DeleteReservation> _deleteReservationHandler;

        public ReservationsController(ICommandHandler<ReserveParkingSpotForVehicle> reserveParkingSpotsForVehicleHandler,
            ICommandHandler<ReserveParkingSpotForCleaning> reserveParkingSpotsForCleaningHandler,
            ICommandHandler<ChangeReservationLicensePlate> changeReservationLicencePlateHandler,
            ICommandHandler<DeleteReservation> deleteReservationHandler)
        {
            _reserveParkingSpotsForVehicleHandler = reserveParkingSpotsForVehicleHandler;
            _reserveParkingSpotsForCleaningHandler = reserveParkingSpotsForCleaningHandler;
            _changeReservationLicencePlateHandler = changeReservationLicencePlateHandler;
            _deleteReservationHandler = deleteReservationHandler;
        }


        [Authorize]
        [HttpPost("{parkingSpotId:guid}/reservations/vehicle")]
        public async Task<ActionResult> Post(Guid parkingSpotId, ReserveParkingSpotForVehicle command)
        {
            string pattern = @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
            Match match = Regex.Match(User.Identity?.Name, pattern);

            string guidString = match.Value;
            var userId = Guid.Parse(guidString);

            await _reserveParkingSpotsForVehicleHandler.HandleAsync(command with
            {
                ReservationId = Guid.NewGuid(),
                ParkingSpotId = parkingSpotId,
                UserId = userId
            });

            return NoContent();
        }

        [HttpPost("reservations/cleaning")]
        public async Task<ActionResult> Post(ReserveParkingSpotForCleaning command)
        {
            await _reserveParkingSpotsForCleaningHandler.HandleAsync(command);
            return NoContent();
        }

        [HttpPut("reservations/{reservationId:guid}")]
        public async Task<ActionResult> Put(Guid reservationId, ChangeReservationLicensePlate command)
        {
            await _changeReservationLicencePlateHandler.HandleAsync(command with { ReservationId = reservationId });
            return NoContent();
        }

        [HttpDelete("reservations/{reservationId:guid}")]
        public async Task<ActionResult> Delete(Guid reservationId)
        {
            await _deleteReservationHandler.HandleAsync(new DeleteReservation(reservationId));
            return NoContent();
        }
    }
}
