namespace BCA_Car_Auction.Models.Vehicles
{
    public enum CarStatus
    {
        Available,
        OnAuction,
        Sold
    }
    public enum CarType
    {
        Hatchback,
        Sedan,
        SUV,
        Truck
    }
    public abstract class Car
    {
        private static int _nextId = 0;
        private readonly object _carLock = new(); // Per-car lock

        public int Id { get; init; }
        public string Manufacturer { get; init; }
        public string Model { get; init; }
        public int Year { get; init; }
        public decimal StartBid { get; init; }
        public CarStatus Status { get; private set; }

        // Thread-safe method to change car status
        public void SetCarOnAuction()
        {
            lock (_carLock)
            {
                Status = CarStatus.OnAuction;
            }
        }

        public void SetCarSold()
        {
            lock (_carLock)
            {
                Status = CarStatus.Sold;
            }
        }
        public void SetCarAvailable()
        {
            lock (_carLock)
            {
                Status = CarStatus.Available;
            }
        }
        public abstract CarType GetCarType();

        public Car(string manufacturer, string model, int year, decimal startBid)
        {
            this.Id = GetNextId();
            this.Status = CarStatus.Available;
            this.Manufacturer = manufacturer;
            this.Model = model;
            this.Year = year;
            this.StartBid = startBid;
            SetCarAvailable();
        }
        protected static int GetNextId() => Interlocked.Increment(ref _nextId); // Thread-safe
    }

    public class Truck : Car
    {
        public double LoadCapacityTons { get; init; }
        public Truck(string manufacturer, string model, int year, decimal startBid, double loadCapacityTons)
        : base(manufacturer, model, year, startBid) { this.LoadCapacityTons = loadCapacityTons; }

        public override CarType GetCarType() => CarType.Truck;
    }
    public class SUV : Car
    {
        public int NumberOfSeats { get; init; }
        public SUV(string manufacturer, string model, int year, decimal startBid, int numberOfSeats)
        : base(manufacturer, model, year, startBid)
        {
            this.NumberOfSeats = numberOfSeats;
        }
        public override CarType GetCarType() => CarType.SUV;
    }

    public class Sedan : Car
    {
        public int NumberOfDoors { get; init; }
        public Sedan(string manufacturer, string model, int year, decimal startBid, int numberOfDoors)
        : base(manufacturer, model, year, startBid)
        {
            this.NumberOfDoors = numberOfDoors;
        }
        public override CarType GetCarType() => CarType.Sedan;
    }

    public class Hatchback : Car
    {
        public int NumberOfDoors { get; init; }
        public Hatchback(string manufacturer, string model, int year, decimal startBid, int numberOfDoors)
        : base(manufacturer, model, year, startBid)
        {
            this.NumberOfDoors = numberOfDoors;
        }
        public override CarType GetCarType() => CarType.Hatchback;
    }

    //another way but I found it kind of a awfull
    //public class CarWithDoors : CarAbstract
    //{
    //    public required int NumberOfDoors { get; init; }
    //}

    //public class Sudan : CarWithDoors { }

    //public class Hatchback : CarWithDoors { }
}
