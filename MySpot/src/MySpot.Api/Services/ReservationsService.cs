using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Services;
using MySpot.Api.Commands;
using MySpot.Api.DTO;
using MySpot.Api.Entities;
using MySpot.Api.ValueObjects;

namespace MySpot.Api.Services{
    public class ReservationsServices {
        private readonly List<WeeklyParkingSpot> _weeklyParkingSpots;
        private readonly Clock _clock = ;

        

        public ReservationsServices(List<WeeklyParkingSpot> weeklyParkingSpots,Clock clock)
        {
            _clock = clock;
            _weeklyParkingSpots = weeklyParkingSpots;
        }

        public ReservationDTO Get(Guid id) => GetAllWeekly().SingleOrDefault(x => x.Id == id);
        public IEnumerable<ReservationDTO> GetAllWeekly() =>
            _weeklyParkingSpots.SelectMany(x => x.Reservations)
            .Select(x => new ReservationDTO
            {
                Id = x.Id,
                ParkingSpotId = x.ParkingSpotId,
                EmployeeName = x.EmployeeName,
                Date = x.Date.Value.Date
            });

        public Guid? Create(CreateReservation command)
        {
            var parkingSpotId = new ParkingSpotId(command.ParkingSpotId);
            var weeklyParkingSpot = _weeklyParkingSpots.SingleOrDefault(x => x.Id == parkingSpotId);
            if(weeklyParkingSpot is null)
            {
                return default;
            }

            var reservation = new Reservation(command.ReservationId, command.ParkingSpotId, command.EmployeeName, command.LicensePlate, new Date(command.Date));
            weeklyParkingSpot.AddReservation(reservation,new Date(Clock.Current()));
            
            return reservation.Id;
        }

        public bool Update(ChangeReservationLicensePlate command)
        {
            var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);
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

            if(existingReservation.Date.Value.Date <= Clock.Current())
            {
                return false;
            }

            existingReservation.ChangeLicensePlate(command.LicensePlate);
            return true;
        }

        public bool Delete(DeleteReservation command)
        {
            var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);
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

            return true;
        }

        private WeeklyParkingSpot GetWeeklyParkingSpotByReservation(ReservationId reservationId)
            => _weeklyParkingSpots.SingleOrDefault(x => x.Reservations.Any(r => r.Id == reservationId));
    }
}
