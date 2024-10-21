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
        public ActionResult<IEnumerable<ReservationDTO>> Get()
        {
            return Ok(_reservationsServices.GetAllWeekly());
        }

        [HttpGet("{id:guid}")]
        public ActionResult<ReservationDTO> GetById(Guid id)
        {
            var reservation = _reservationsServices.Get(id);
            if(reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpPost]
        public ActionResult Post(CreateReservation command) 
        {

            var id = _reservationsServices.Create(command with { ReservationId = Guid.NewGuid()});
            if(id == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(Get),new {id},null);
        }

        [HttpPut("{id:guid}")]
        public ActionResult Put(Guid id,ChangeReservationLicensePlate command)
        {
            if(_reservationsServices.Update(command with { ReservationId = id}))
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id:guid}")]
        public ActionResult Delete (Guid id)
        {
            if(_reservationsServices.Delete(new DeleteReservation(id)))
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
