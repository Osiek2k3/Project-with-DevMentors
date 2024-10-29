using MySpot.Application.Commands;
using MySpot.Application.DTO;

namespace MySpot.Application.Services
{
    public interface IReservationsServices
    {
        Task<ReservationDTO> GetAsync(Guid id);
        Task<IEnumerable<ReservationDTO>> GetAllWeeklyAsync();
        Task<Guid?> ReserveForVehicleAsync(ReserveParkingSpotForVehicle command);
        Task ReserveForCleaningAsync(ReserveParkingSpotForCleaning command);
        Task<bool> ChangeReservationLicensePlateAsync(ChangeReservationLicensePlate command);
        Task<bool> DeleteAsync(DeleteReservation command);
    }
}
