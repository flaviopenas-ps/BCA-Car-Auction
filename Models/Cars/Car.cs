using System.Runtime.Serialization;

namespace BCA_Car_Auction.Models.Vehicles
{
    public enum CarStatus
    {
        [EnumMember(Value = "Available")]
        Available,
        [EnumMember(Value = "OnAuction")]
        OnAuction,
        [EnumMember(Value = "Sold")]
        Sold
    }
    public enum CarType
    {
        [EnumMember(Value = "Hatchback")]
        Hatchback = 0,
        [EnumMember(Value = "Sedan")]
        Sedan = 1,
        [EnumMember(Value = "SUV")]
        SUV = 2,
        [EnumMember(Value = "Truck")]
        Truck = 3
    }
    public abstract class Car : ICloneable
    {
        private static int _nextId = 0;
        private readonly object _carLock = new(); // Per-car lock

        public int Id { get; protected set; }
        public int UserIdOwner { get; init; } // UserId owner of the car
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

        public abstract object Clone();

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

        protected static void SetClonedProperties(Car target, Car source)
        {
            var idField = typeof(Car).GetProperty(nameof(Id));
            if (idField != null && idField.CanWrite)
                idField.SetValue(target, source.Id);

            target.Status = source.Status;
        }

        public Car(string manufacturer, string model, int year, decimal startBid, int userIdOwner)
        {
            this.Id = GetNextId();
            this.Status = CarStatus.Available;
            this.Manufacturer = manufacturer;
            this.Model = model;
            this.Year = year;
            this.StartBid = startBid;
            this.UserIdOwner = userIdOwner;
            SetCarAvailable();
        }

        protected static int GetNextId() => Interlocked.Increment(ref _nextId); // Thread-safe
    }

    public class Truck : Car
    {
        public double LoadCapacityTons { get; init; }
        public Truck(string manufacturer, string model, int year, 
            decimal startBid, double loadCapacityTons, int userIdOwner)
        : base(manufacturer, model, year, startBid, userIdOwner) 
        { 
            this.LoadCapacityTons = loadCapacityTons;
        }
        public override object Clone()
        {
            var clone = new Truck(Manufacturer, Model, Year, StartBid, LoadCapacityTons, UserIdOwner);
            SetClonedProperties(clone, this);
            return clone;
        }

        public override CarType GetCarType() => CarType.Truck;
    }
    public class SUV : Car
    {
        public int NumberOfSeats { get; init; }
        public SUV(string manufacturer, string model, int year, 
            decimal startBid, int numberOfSeats, int userIdOwner)
        : base(manufacturer, model, year, startBid, userIdOwner)
        {
            this.NumberOfSeats = numberOfSeats;
        }

        public override object Clone()
        {
            var clone = new SUV(Manufacturer, Model, Year, StartBid, NumberOfSeats, UserIdOwner);
            SetClonedProperties(clone, this);
            return clone;
        }

        public override CarType GetCarType() => CarType.SUV;
    }

    public class Sedan : Car
    {
        public int NumberOfDoors { get; init; }
        public Sedan(string manufacturer, string model, int year, 
            decimal startBid, int numberOfDoors, int userIdOwner)
        : base(manufacturer, model, year, startBid, userIdOwner)
        {
            this.NumberOfDoors = numberOfDoors;
        }

        public override object Clone()
        {
            var clone = new Sedan(Manufacturer, Model, Year, StartBid, NumberOfDoors, UserIdOwner);
            SetClonedProperties(clone, this);
            return clone;
        }

        public override CarType GetCarType() => CarType.Sedan;
    }

    public class Hatchback : Car
    {
        public int NumberOfDoors { get; init; }
        public Hatchback(string manufacturer, string model, int year, 
            decimal startBid, int numberOfDoors, int userIdOwner)
        : base(manufacturer, model, year, startBid, userIdOwner)
        {
            this.NumberOfDoors = numberOfDoors;
        }

        public override object Clone()
        {
            var clone = new Hatchback(Manufacturer, Model, Year, StartBid, NumberOfDoors, UserIdOwner);
            SetClonedProperties(clone, this);
            return clone;
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
