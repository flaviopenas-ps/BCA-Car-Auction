using BCA_Car_Auction.DTOs.Cars;
using BCA_Car_Auction.Models.Auctions;
using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Validation;
using System.Collections;
using System.Collections.Concurrent;

namespace BCA_Car_Auction.Services
{

    public interface ICarService
    {
        Car AddCar(CarRequest request);
        List<Car> SearchCars(CarType? type = null, CarStatus? carStatus = null,
            string? manufacturer = null, string? model = null, int? year = null);
        void MarkAsAvailable(int carId);

        void MarkAsOnAuction(int carId);

        void MarkAsSold(int carId);

        Car GetCarByIdAvailableByRef(int carId);

        Car GetCarByIdOnAuctionByRef(int carId);
        public int GetUserId(int carId);
        decimal GetStartBid(int carId);
        void Reset();//tests
    }

    public class CarService : ICarService
    {
        private readonly ConcurrentDictionary<int, Car> _inventory = new();
        private readonly ICarFactory _factory;
        private readonly IUserService _users;

        public CarService(ICarFactory factory, IUserService users)
        {
            _factory = factory;
            _users = users;
        }
        //return a reference to the car
        public Car AddCar(CarRequest request)
        {
            _users.GetUserById(request.UserIdOwner);

            var car = _factory.Create(request);
            _inventory.TryAdd(car.Id, car);

            return car;
        }
        //return a reference to the car
        public Car GetCarByIdAvailableByRef(int carId)
        {
            Car? car;
            _inventory.TryGetValue(carId, out car);

            car.ThrowIfNull("Car not found");

            car.ThrowIfCarNotFound();
            car.ThrowIfCarNotAvaiable();

            return car;

        }
        //return a reference to the car
        public Car GetCarByIdOnAuctionByRef(int carId)
        {
            Car? car;
            _inventory.TryGetValue(carId, out car);

            car.ThrowIfNull("Car not found");

            car.ThrowIfCarNotFound();
            car.ThrowIfCarNotOnAuction();

            return car;

        }
        //returns a copy, not a reference
        public List<Car> SearchCars(CarType? type = null, CarStatus? carStatus = null,
            string? manufacturer = null, string? model = null, int? year = null)
        {
            return _inventory.Values
                .Where(c => !carStatus.HasValue || c.Status == carStatus)
                .Where(c => !type.HasValue || c.GetCarType() == type.Value)
                .Where(c => string.IsNullOrWhiteSpace(manufacturer) || c.Manufacturer.Equals(manufacturer, StringComparison.OrdinalIgnoreCase))
                .Where(c => string.IsNullOrWhiteSpace(model) || c.Model.Equals(model, StringComparison.OrdinalIgnoreCase))
                .Where(c => !year.HasValue || c.Year == year.Value)
                .Select(c => (Car)c.Clone())
                .ToList();
        }

        public int GetUserId(int carId)
        {
            Car? car;
            _inventory.TryGetValue(carId, out car);
            car.ThrowIfNull("Car not found");

            return car.UserIdOwner;
        }

        public decimal GetStartBid(int carId)
        {
            Car? car;
            _inventory.TryGetValue(carId, out car);
            car.ThrowIfNull("Car not found");

            return car.StartBid;
        }

        public void MarkAsAvailable(int carId)
        {
            Car? car;
            _inventory.TryGetValue(carId, out car);

            car.ThrowIfNull("Car not found");
            car.SetCarAvailable();
        }

        public void MarkAsSold(int carId)
        {
            Car? car;
            _inventory.TryGetValue(carId, out car);

            car.ThrowIfNull("Car not found");
            car.SetCarSold();
        }

        public void MarkAsOnAuction(int carId)
        {
            Car? car;
            _inventory.TryGetValue(carId, out car);

            car.ThrowIfNull("Car not found");
            car.SetCarOnAuction();
        }

        public void Reset()
        {
            _inventory.Clear();
        }
    }
}
