﻿using My.Spot.Tests.Unit.Shared;
using MySpot.Application.Commands;
using MySpot.Application.Services;
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
            var command = new CreateReservation(parkingSpot.Id, Guid.NewGuid(),
                DateTime.UtcNow.AddMinutes(5), "John Doe", "XYZ123");

            var reservationId = await _reservationService.CreateAsync(command);

            reservationId.ShouldNotBeNull();
            reservationId.Value.ShouldBe(command.ReservationId);
        }

        #region Arrange

        private readonly IClock _clock = new TestClock();
        private readonly IReservationsServices _reservationService;
        private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;

        public ReservationServiceTests() 
        {
            _weeklyParkingSpotRepository = new InMemoryWeeklyParkingSpotRepository(_clock);
            _reservationService = new ReservationsServices(_clock, _weeklyParkingSpotRepository);
        }

        #endregion
    }
}
