using BCA_Car_Auction.DTOs;
using BCA_Car_Auction.DTOs.Cars;
using BCA_Car_Auction.DTOs.Users;
using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Services;
using Xunit;

namespace BCA_Car_Auction.Tests
{
    public class CarServiceFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }

        public CarServiceFixture()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ICarFactory, CarFactory>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ICarService, CarService>();
            services.AddSingleton<IAuctionService, AuctionService>();

            ServiceProvider = services.BuildServiceProvider();
        }
        public void Dispose()
        {
            // Clean up if needed
        }
    }

    public class CarServiceTests : IClassFixture<CarServiceFixture>
    {
        private readonly ICarService _carService;
        private readonly IUserService _userService;
        private readonly ICarFactory _carFactory;

        public CarServiceTests(CarServiceFixture fixture)
        {
            _carService = fixture.ServiceProvider.GetRequiredService<ICarService>();
            _userService = fixture.ServiceProvider.GetRequiredService<IUserService>();
            _carFactory = fixture.ServiceProvider.GetRequiredService<ICarFactory>();

            // Clear state before each test
            _carService.Reset();
            _userService.Reset();
        }

        private CarRequest CreateValidCarRequestTruck(int ownerId, CarType type = CarType.Truck)
        {
            return new CarRequest
            {
                Manufacturer = "Toyota",
                Model = "Camry",
                Year = 2022,
                StartBid = 25000,
                UserIdOwner = ownerId,
                Type = type,
                LoadCapacityTons = 4.1 // For Truck
            };
        }

        private CarRequest CreateValidCarRequestDoors(int ownerId, CarType type = CarType.Sedan)
        {
            return new CarRequest
            {
                Manufacturer = "Toyota",
                Model = "Camry",
                Year = 2022,
                StartBid = 25000,
                UserIdOwner = ownerId,
                Type = type,
                NumberOfDoors = 4 // For Sedan/Hatchback
            };
        }

        [Fact]
        public void AddCar_WithValidRequest_ShouldAddCarToInventory()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var request = CreateValidCarRequestDoors(owner.Id);

            var car = _carService.AddCar(request);

            Assert.NotNull(car);
            Assert.Equal(request.Manufacturer, car.Manufacturer);
            Assert.Equal(request.Model, car.Model);
            Assert.Equal(request.Year, car.Year);
            Assert.Equal(CarStatus.Available, car.Status);

            var retrievedCar = _carService.GetCarByIdAvailableByRef(car.Id);
            Assert.Equal(car, retrievedCar);
        }

        [Fact]
        public void AddCar_WithInvalidOwner_ShouldThrowException()
        {
            var request = CreateValidCarRequestDoors(999); // Non-existent owner

            Assert.Throws<ArgumentNullException>(() => _carService.AddCar(request));
        }

        [Fact]
        public void GetCarByIdAvailableByRef_ForAvailableCar_ShouldReturnCar()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            var result = _carService.GetCarByIdAvailableByRef(car.Id);

            Assert.Equal(car, result);
        }

        [Fact]
        public void GetCarByIdAvailableByRef_ForNonAvailableCar_ShouldThrowException()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            car.SetCarOnAuction();

            var ex = Assert.Throws<Exception>(() => _carService.GetCarByIdAvailableByRef(car.Id));
            Assert.Contains("Car should be in an avaiable state.", ex.Message);
        }

        [Fact]
        public void GetCarByIdAvailableByRef_ForNonExistentCar_ShouldThrowException()
        {
            var nonExistentCarId = 9999;

            var ex = Assert.Throws<ArgumentNullException>(() => _carService.GetCarByIdAvailableByRef(nonExistentCarId));
            Assert.Contains("Car not found", ex.Message);
        }

        [Fact]
        public void GetCarByIdOnAuctionByRef_ForCarOnAuction_ShouldReturnCar()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            car.SetCarOnAuction();

            var result = _carService.GetCarByIdOnAuctionByRef(car.Id);

            Assert.Equal(car, result);
        }

        [Fact]
        public void GetCarByIdOnAuctionByRef_ForAvailableCar_ShouldThrowException()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            var ex = Assert.Throws<Exception>(() => _carService.GetCarByIdOnAuctionByRef(car.Id));
            Assert.Contains("Car should be in an auction state", ex.Message);
        }

        [Fact]
        public void MarkAsAvailable_ShouldChangeCarStatus()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            car.SetCarOnAuction();

            _carService.MarkAsAvailable(car);

            Assert.Equal(CarStatus.Available, car.Status);
        }

        [Fact]
        public void MarkAsSold_ShouldChangeCarStatus()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestTruck(owner.Id));
            car.SetCarOnAuction();

            _carService.MarkAsSold(car);

            Assert.Equal(CarStatus.Sold, car.Status);
        }

        [Fact]
        public void SearchCars_ShouldFilterByType()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var sedan = _carService.AddCar(CreateValidCarRequestDoors(owner.Id, CarType.Sedan));
            var suv = _carService.AddCar(CreateValidCarRequestTruck(owner.Id, CarType.Truck));

            var results = _carService.SearchCars(type: CarType.Sedan);

            Assert.Single(results);
            Assert.Equal(sedan.Id, results[0].Id);
        }

        [Fact]
        public void SearchCars_ShouldFilterByStatusAuction()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var availableCar = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            var auctionCar = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            auctionCar.SetCarOnAuction();

            var results = _carService.SearchCars(carStatus: CarStatus.OnAuction);

            Assert.Single(results);
            Assert.Equal(auctionCar.Id, results[0].Id);
        }

        [Fact]
        public void SearchCars_ShouldFilterByManufacturer()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var toyota = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            var hondaRequest = CreateValidCarRequestDoors(owner.Id);
            hondaRequest.Manufacturer = "Honda";
            _carService.AddCar(hondaRequest);

            var results = _carService.SearchCars(manufacturer: "Toyota");

            Assert.Single(results);
            Assert.Equal(toyota.Id, results[0].Id);
        }

        [Fact]
        public void GetAllCars_ShouldReturnAllCars()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car1 = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            var car2 = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            var results = _carService.SearchCars();

            Assert.Equal(2, results.Count);
            Assert.Contains(car1, results);
            Assert.Contains(car2, results);
        }

        [Fact]
        public void Reset_ShouldClearInventory()
        {
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            _carService.Reset();

            Assert.Empty(_carService.SearchCars());
        }
    }
}
