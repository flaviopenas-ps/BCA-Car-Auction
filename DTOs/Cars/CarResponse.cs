using BCA_Car_Auction.Models.Vehicles;

public class CarResponse
{
    public int Id { get; set; }
    public CarType Type { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal StartBid { get; set; }
    public CarStatus Status { get; set; }
    public int UserIdOwner { get; set; }

    public int? NumberOfDoors { get; set; }
    public int? NumberOfSeats { get; set; }
    public double? LoadCapacityTons { get; set; }

    public static CarResponse FromCar(Car car)
    {
        var response = new CarResponse
        {
            Id = car.Id,
            Type = car.GetCarType(),
            Manufacturer = car.Manufacturer,
            Model = car.Model,
            Year = car.Year,
            StartBid = car.StartBid,
            Status = car.Status,
            UserIdOwner = car.UserIdOwner
        };

        switch (car)
        {
            case Hatchback hatch:
                response.NumberOfDoors = hatch.NumberOfDoors;
                break;
            case Sedan sedan:
                response.NumberOfDoors = sedan.NumberOfDoors;
                break;
            case SUV suv:
                response.NumberOfSeats = suv.NumberOfSeats;
                break;
            case Truck truck:
                response.LoadCapacityTons = truck.LoadCapacityTons;
                break;
        }

        return response;
    }
}