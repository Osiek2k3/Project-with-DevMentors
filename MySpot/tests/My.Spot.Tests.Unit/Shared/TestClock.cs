using MySpot.Core.Abstractions;

namespace My.Spot.Tests.Unit.Shared
{
    public class TestClock : IClock
    {
        public DateTime Current() => new DateTime(2024, 10, 29,12,0,0);
    }
}
