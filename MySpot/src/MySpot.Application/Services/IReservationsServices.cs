using MySpot.Application.Commands;
using MySpot.Application.DTO;

namespace MySpot.Application.Services
{
    public interface IReservationsServices
    {
        ReservationDTO Get(Guid id);
        IEnumerable<ReservationDTO> GetAllWeekly();
        Guid? Create(CreateReservation command);
        bool Update(ChangeReservationLicensePlate command);
        bool Delete(DeleteReservation command);
    }
}
