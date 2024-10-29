using My.Spot.Tests.Unit.Shared;
using MySpot.Application.Commands;
using MySpot.Application.Services;
using MySpot.Core.Abstractions;
using MySpot.Core.DoaminServices;
using MySpot.Core.Policies;
using MySpot.Core.Repositories;
using MySpot.Infrastructure.DAL.Repositories;
using Shouldly;

namespace My.Spot.Tests.Unit.Services
{
    public class ReservationServiceTests
    {
        [Fact]
        public async Task given_reservation_for_not_taken_date_create_reservation_should_succeed()
        {
            var parkingSpot = (await _weeklyParkingSpotRepository.GetAllAsync()).First();
            var command = new ReserveParkingSpotForVehicle(parkingSpot.Id, Guid.NewGuid(),2,
                DateTime.UtcNow.AddMinutes(5), "John Doe", "XYZ123");

            var reservationId = await _reservationService.ReserveForVehicleAsync(command);

            reservationId.ShouldNotBeNull();
            reservationId.Value.ShouldBe(command.ReservationId);
        }

        #region Arrange

        private readonly IClock _clock ;
        private readonly IReservationsServices _reservationService;
        private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;
        

        public ReservationServiceTests() 
        {
            _clock = new TestClock();
            _weeklyParkingSpotRepository = new InMemoryWeeklyParkingSpotRepository(_clock);

            var prakingReservationService = new ParkingReservationService(new IReservationPolicy[]
            {
                new BossReservationPolicy(),
                new ManagerReservationPolicy(),
                new RegularEmployeeReservationPolicy(_clock)

            },_clock);

            _reservationService = new ReservationsServices(_clock, _weeklyParkingSpotRepository, prakingReservationService);
        }

        #endregion
    }
}
