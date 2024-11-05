using MySpot.Application.Abstractions;
using MySpot.Core.DoaminServices;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Application.Commands.Handlers
{
    public sealed class ReserveParkingSpotForCleaningHandler : ICommandHandler<ReserveParkingSpotForCleaning>
    {
        private readonly IWeeklyParkingSpotRepository _repository;
        private readonly IParkingReservationService _reservationService;

        public ReserveParkingSpotForCleaningHandler(IWeeklyParkingSpotRepository repository,
            IParkingReservationService reservationService)
        {
            _repository = repository;
            _reservationService = reservationService;
        }

        public async Task HandleAsync(ReserveParkingSpotForCleaning command)
        {
            var week = new Week(command.date);
            var weeklyParkingSpots = (await _repository.GetByWeekAsync(week)).ToList();

            _reservationService.ReserveParkingForCleaning(weeklyParkingSpots, new Date(command.date));

            var tasks = weeklyParkingSpots.Select(x => _repository.UpdateAsync(x));
            await Task.WhenAll(tasks);
            /*foreach (var spot in weeklyParkingSpots)
            {
                await _repository.UpdateAsync(spot);
            }*/
        }
    }

}