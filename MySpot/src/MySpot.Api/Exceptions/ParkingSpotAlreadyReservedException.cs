namespace MySpot.Api.Exception
{
    public sealed class ParkingSpotAlreadyReservedException : CustomException
    {
        public string Name {get;}
        public DateTime Date {get;}
        public ParkingSpotAlreadyReservedException(string name,DateTime date) : 
        base($"Parking spot: {name} is already reserved at: {date:d}")
        {
            Date = date;
            Name = name;
        }
    }
}