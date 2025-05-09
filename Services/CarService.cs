using BCA_Car_Auction.DTOs;
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
        void MarkAsAvailable(Car car);

        void MarkAsSold(Car car);

        Car GetCarByIdAvailableByRef(int carId);

        Car GetCarByIdOnAuctionByRef(int carId);

        List<Car> GetAllCars();
    }

    public class CarService : ICarService
    {
        private readonly ConcurrentDictionary<int, Car> _inventory = new();
        private readonly ICarFactory _factory;
        private readonly ConcurrentDictionary<int, object> _carLocks = new();

        public CarService(ICarFactory factory)
        {
            _factory = factory;
        }

        public Car AddCar(CarRequest request)
        {
            var car = _factory.Create(request);
            _inventory.TryAdd(car.Id, car);
            _carLocks[car.Id] = new object();

            return car;
        }

        public List<Car> GetAllCars()
        {
            return _inventory.Values.ToList();
        }

        public Car GetCarByIdAvailableByRef(int carId)
        {
            Car car = _inventory[carId].ThrowIfNull($"Car with ID {carId} not found");

            car.ValidateNotOnAuction();

            car.ValidateNotSold();

            return car;

        }

        public Car GetCarByIdOnAuctionByRef(int carId)
        {
            Car car = _inventory[carId].ThrowIfNull($"Car with ID {carId} not found");

            car.ValidateNotOnAuction();

            car.ValidateNotSold();

            return car;

        }


        public List<Car> SearchCars(CarType? type = null, CarStatus? carStatus = null,
            string? manufacturer = null, string? model = null, int? year = null)
        {
            return _inventory.Values
                .Where(c => !carStatus.HasValue || c.Status == carStatus)
                .Where(c => !type.HasValue || c.GetCarType() == type.Value)
                .Where(c => string.IsNullOrWhiteSpace(manufacturer) || c.Manufacturer.Equals(manufacturer, StringComparison.OrdinalIgnoreCase))
                .Where(c => string.IsNullOrWhiteSpace(model) || c.Model.Equals(model, StringComparison.OrdinalIgnoreCase))
                .Where(c => !year.HasValue || c.Year == year.Value).ToList();
        }

        public void MarkAsAvailable(Car car)
        {
            car.SetCarAvailable();
        }

        public void MarkAsSold(Car car)
        {
            car.SetCarSold();
        }
    }
}
