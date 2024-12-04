
using MySpot.Core.ValueObjects;

namespace MySpot.Core.Entities
{
    public sealed class CleaningReservation : Reservation
    {
        private CleaningReservation()
        {
            
        }
        public CleaningReservation(ReservationId id, Date date) 
            : base(id, 2, date)
        {
        }
    }
}
