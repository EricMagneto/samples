using System;

namespace FlightFinder.Shared
{
    public class RentalCarSearchCriteria
    {
        public string Airport { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public RentalCarKind Kind { get; set; }

        public RentalCarSearchCriteria(string airport)
        {
            Airport = airport;
            PickupDate = DateTime.Now.Date;
            ReturnDate = PickupDate.AddDays(7);
        }
    }
}
