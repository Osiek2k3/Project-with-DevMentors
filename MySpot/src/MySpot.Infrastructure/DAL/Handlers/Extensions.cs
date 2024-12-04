using MySpot.Application.DTO;
using MySpot.Core.Entities;

namespace MySpot.Infrastructure.DAL.Handlers
{
    internal static class Extensions
    {
        public static WeeklyParkingSpotDto AsDto(this WeeklyParkingSpot entity)
            => new ()
            {
                Id = entity.Id.Value.ToString(),
                Name = entity.Name,
                Capacity = entity.Capacity,
                From = entity.Week.From.Value.DateTime,
                To = entity.Week.To.Value.DateTime,
                Reservations = entity.Reservations.Select(x => new ReservationDTO
                {
                    Id = x.Id,
                    EmployeeName = x is VehicleReservation vr ? vr.EmployeeName : null,
                    Date = x.Date.Value.Date
                })
            };

        public static UserDto AsDto(this User entity)
        => new()
        {
            Id = entity.Id,
            Username = entity.Username,
            FullName = entity.FullName
        };
    }
}
