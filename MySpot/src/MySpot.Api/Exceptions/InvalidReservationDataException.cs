namespace MySpot.Api.Exception
{
    public sealed class InvalidReservationDataException : CustomException
    {
        public DateTime Date {get;}
        public InvalidReservationDataException(DateTime date) : base($"Reservation date: {date:d} is invalid.")
        {
            Date = date;
        }
    }
}