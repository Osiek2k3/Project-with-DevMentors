using MySpot.Core.Exceptions;

namespace MySpot.ACorepi.Exceptions
{
    public sealed class ReservationNotFoundException : CustomException

    {
        public Guid ID { get; }
        public ReservationNotFoundException(Guid Id) : base($"Reservation Id: {Id} is invalid.")
        {
            ID = Id;
        }
    }
}
