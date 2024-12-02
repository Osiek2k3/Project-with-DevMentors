
using MySpot.Core.ValueObjects;

namespace MySpot.Core.Entities
{
    public sealed class VehicleReservation : Reservation
    {
        public UserId UserId { get;private set; }
        public EmployeeName? EmployeeName { get; private set; }
        public LicensePlate LicensePlate { get; private set; }

        private VehicleReservation()
        {
            
        }

        public VehicleReservation(ReservationId id,UserId userId,
            EmployeeName employeeName, LicensePlate licensePlate,Capacity capacity, Date date) 
            : base(id, capacity, date)
        {
            UserId = userId;
            EmployeeName = employeeName;
            ChangeLicensePlate(licensePlate);
        }

        public void ChangeLicensePlate(LicensePlate licensePlate)
        {
            LicensePlate = licensePlate;
        }
    }
}
