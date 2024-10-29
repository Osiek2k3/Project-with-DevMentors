using System.Data;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Core.Abstractions;
using MySpot.Core.DoaminServices;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Application.Services{
    public class ReservationsServices : IReservationsServices
    {
        private readonly IClock _clock;
        private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;
        private readonly IParkingReservationService _parkingReservationService;

        public ReservationsServices(IClock clock,IWeeklyParkingSpotRepository weeklyParkingSpotRepository,
            IParkingReservationService parkingReservationService)
        {
            _clock = clock;
            _weeklyParkingSpotRepository = weeklyParkingSpotRepository;
            _parkingReservationService = parkingReservationService;
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
                EmployeeName = x is VehicleReservation vr ? vr.EmployeeName : string.Empty,
                Date = x.Date.Value.Date
            });
        }
           

        public async Task<Guid?> ReserveForVehicleAsync(ReserveParkingSpotForVehicle command)
        {
            var parkingSpotId = new ParkingSpotId(command.ParkingSpotId);
            var week = new Week(_clock.Current());

            var weeklyParkingSpots = (await _weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();
            var parkingSpotToReserve = weeklyParkingSpots.SingleOrDefault(x => x.Id == parkingSpotId);
            if (parkingSpotToReserve is null)
            {
                return default;
            }

            var reservation = new VehicleReservation(command.ReservationId, command.ParkingSpotId, command.EmployeeName,
                command.LicensePlate,command.Capacity, new Date(command.Date));


            _parkingReservationService.ReserveSpotForVehicle(weeklyParkingSpots,JobTitle.Employee,parkingSpotToReserve,reservation);
            await _weeklyParkingSpotRepository.UpdateAsync(parkingSpotToReserve);
            
            return reservation.Id;
        }

        public async Task ReserveForCleaningAsync(ReserveParkingSpotForCleaning command)
        {
            var week = new Week(command.date);
            var weeklyParkingSpots = (await _weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();

            _parkingReservationService.ReserveParkingForCleaning(weeklyParkingSpots,new Date(command.date));

            foreach(var parkingSpot in weeklyParkingSpots)
            {
                await _weeklyParkingSpotRepository.UpdateAsync(parkingSpot);
            }

        }

        public async Task<bool> ChangeReservationLicensePlateAsync(ChangeReservationLicensePlate command)
        {
            var weeklyParkingSpot = await GetWeeklyParkingSpotByReservationAsync(command.ReservationId);
            if(weeklyParkingSpot is null) 
            { 
                return false; 
            }

            var reservationId = new ReservationId(command.ReservationId);
            var existingReservation = weeklyParkingSpot.Reservations
                .OfType<VehicleReservation>()
                .SingleOrDefault(x => x.Id == reservationId);

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

            weeklyParkingSpot.RemoveReservation(existingReservation.Id);
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
