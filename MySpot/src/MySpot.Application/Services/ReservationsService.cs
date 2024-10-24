using System.Data;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Application.Services{
    public class ReservationsServices : IReservationsServices
    {
        private readonly IClock _clock;
        private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;

        public ReservationsServices(IClock clock,IWeeklyParkingSpotRepository weeklyParkingSpotRepository)
        {
            _clock = clock;
            _weeklyParkingSpotRepository = weeklyParkingSpotRepository;
        }

        public async Task<ReservationDTO> GetAsync(Guid id)
        {
            var reservations = await GetAllWeeklyAsync();
            return reservations.SingleOrDefault(x => x.Id == id);
        }
        public async Task<IEnumerable<ReservationDTO>> GetAllWeeklyAsync()
        {
            var weeklyParkingSpots = await _weeklyParkingSpotRepository.GetAllAsync();

            return weeklyParkingSpots
            .SelectMany(x => x.Reservations)
            .Select(x => new ReservationDTO
            {
                Id = x.Id,
                ParkingSpotId = x.ParkingSpotId,
                EmployeeName = x.EmployeeName,
                Date = x.Date.Value.Date
            });
        }
           

        public async Task<Guid?> CreateAsync(CreateReservation command)
        {
            var parkingSpotId = new ParkingSpotId(command.ParkingSpotId);
            var weeklyParkingSpot = await _weeklyParkingSpotRepository.GetAsync(parkingSpotId);
            if(weeklyParkingSpot is null)
            {
                return default;
            }

            var reservation = new Reservation(command.ReservationId, command.ParkingSpotId, command.EmployeeName, command.LicensePlate, new Date(command.Date));
            weeklyParkingSpot.AddReservation(reservation,new Date(_clock.Current()));
            await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);
            
            return reservation.Id;
        }

        public async Task<bool> UpdateAsync(ChangeReservationLicensePlate command)
        {
            var weeklyParkingSpot = await GetWeeklyParkingSpotByReservationAsync(command.ReservationId);
            if(weeklyParkingSpot is null) 
            { 
                return false; 
            }

            var reservationId = new ReservationId(command.ReservationId);
            var existingReservation = weeklyParkingSpot.Reservations.SingleOrDefault(x => x.Id == reservationId);
            if(existingReservation == null)
            {
                return false;
            }

            if(existingReservation.Date.Value.Date <= _clock.Current())
            {
                return false;
            }

            existingReservation.ChangeLicensePlate(command.LicensePlate);

            await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);
            return true;
        }

        public async Task<bool> DeleteAsync(DeleteReservation command)
        {
            var weeklyParkingSpot = await GetWeeklyParkingSpotByReservationAsync(command.ReservationId);
            if (weeklyParkingSpot is null)
            {
                return false;
            }

            var reservationId = new ReservationId(command.ReservationId);
            var existingReservation = weeklyParkingSpot.Reservations.SingleOrDefault(x => x.Id == reservationId);
            if(existingReservation == null)
            {
                return false;
            }

            weeklyParkingSpot.RemoveReservation(existingReservation);
            await _weeklyParkingSpotRepository.DeleteAsync(weeklyParkingSpot);

            return true;
        }

        private async Task<WeeklyParkingSpot> GetWeeklyParkingSpotByReservationAsync(ReservationId reservationId)
        {
            var weeklyParkingSpot = await _weeklyParkingSpotRepository.GetAllAsync();

            return weeklyParkingSpot
                .SingleOrDefault(x => x.Reservations.Any(r => r.Id == reservationId));
        }
    }
}
