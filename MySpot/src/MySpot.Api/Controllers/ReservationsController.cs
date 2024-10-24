using Microsoft.AspNetCore.Mvc;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Services;

namespace MySpot.Api.Controllers
{
    [ApiController]
    [Route("reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsServices _reservationsServices;

        public ReservationsController(IReservationsServices reservationsServices) 
        {
            _reservationsServices = reservationsServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> Get()
        {
            return Ok(await _reservationsServices.GetAllWeeklyAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ReservationDTO>> GetById(Guid id)
        {
            var reservation = await _reservationsServices.GetAsync(id);
            if(reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateReservation command) 
        {

            var id = await _reservationsServices.CreateAsync(command with { ReservationId = Guid.NewGuid()});
            if(id == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(Get),new {id},null);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Put(Guid id,ChangeReservationLicensePlate command)
        {
            if(await _reservationsServices.UpdateAsync(command with { ReservationId = id}))
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete (Guid id)
        {
            if(await _reservationsServices.DeleteAsync(new DeleteReservation(id)))
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
