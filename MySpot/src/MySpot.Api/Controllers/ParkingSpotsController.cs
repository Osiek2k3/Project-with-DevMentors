using Microsoft.AspNetCore.Mvc;
using MySpot.Application.Abstractions;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Queries;


namespace MySpot.Api.Controllers
{
    [ApiController]
    [Route("parking-spots")]
    public class ParkingSpotsController : ControllerBase
    {
        private readonly ICommandHandler<ReserveParkingSpotForVehicle> _reserveParkingSpotsForVehicleHandler;
        private readonly ICommandHandler<ReserveParkingSpotForCleaning> _reserveParkingSpotsForCleaningHandler;
        private readonly ICommandHandler<ChangeReservationLicensePlate> _changeReservationLicensePlateHandler;
        private readonly ICommandHandler<DeleteReservation> _deleteReservationHandler;
        private readonly IQueryHandler<GetWeeklyParkingSpots, IEnumerable<WeeklyParkingSpotDto>> _getWeeklyParkingSpotsHandler;

        public ParkingSpotsController(ICommandHandler<ReserveParkingSpotForVehicle> reserveParkingSpotsForVehicleHandler,
            ICommandHandler<ReserveParkingSpotForCleaning> reserveParkingSpotsForCleaningHandler,
            ICommandHandler<ChangeReservationLicensePlate> changeReservationLicensePlateHandler,
            ICommandHandler<DeleteReservation> deleteReservationHandler,
            IQueryHandler<GetWeeklyParkingSpots, IEnumerable<WeeklyParkingSpotDto>> getWeeklyParkingSpotsHandler)
        {
            _reserveParkingSpotsForVehicleHandler = reserveParkingSpotsForVehicleHandler;
            _reserveParkingSpotsForCleaningHandler = reserveParkingSpotsForCleaningHandler;
            _changeReservationLicensePlateHandler = changeReservationLicensePlateHandler;
            _deleteReservationHandler = deleteReservationHandler;
            _getWeeklyParkingSpotsHandler = getWeeklyParkingSpotsHandler;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> Get([FromQuery] GetWeeklyParkingSpots query)
            => Ok(await _getWeeklyParkingSpotsHandler.HandleAsync(query));


        [HttpPost("{parkingSpotId:guid}/reservations/vehicle")]
        public async Task<ActionResult> Post(Guid parkingSpotId, ReserveParkingSpotForVehicle command) 
        {
            await _reserveParkingSpotsForVehicleHandler.HandleAsync(command with
            {
                ReservationId = Guid.NewGuid(),
                ParkingSpotId = parkingSpotId
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
            await _changeReservationLicensePlateHandler.HandleAsync(command with
            {
                ReservationId = reservationId
            });
            return NoContent();
        }

        [HttpDelete("reservations/{reservationId:guid}")]
        public async Task<ActionResult> Delete (Guid reservationId)
        {
            await _deleteReservationHandler.HandleAsync(new DeleteReservation(reservationId));
            return NoContent();
        }
    }
}
