using MySpot.Api.Exception;

namespace MySpot.Api.Exceptions
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
