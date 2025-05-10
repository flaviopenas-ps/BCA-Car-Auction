using BCA_Car_Auction.DTOs;
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
                LoadCapacityTons = 4.1 // For Sedan/Hatchback
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
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var request = CreateValidCarRequestDoors(owner.Id);

            // Act
            var car = _carService.AddCar(request);

            // Assert
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
            // Arrange
            var request = CreateValidCarRequestDoors(999); // Non-existent owner

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _carService.AddCar(request));
        }

        [Fact]
        public void GetCarByIdAvailableByRef_ForAvailableCar_ShouldReturnCar()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            // Act
            var result = _carService.GetCarByIdAvailableByRef(car.Id);

            // Assert
            Assert.Equal(car, result);
        }

        [Fact]
        public void GetCarByIdAvailableByRef_ForNonAvailableCar_ShouldThrowException()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            car.SetCarOnAuction();

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _carService.GetCarByIdAvailableByRef(car.Id));
            Assert.Contains("Car should be in an avaiable state.", ex.Message);
        }

        [Fact]
        public void GetCarByIdAvailableByRef_ForNonExistentCar_ShouldThrowException()
        {
            // Arrange
            var nonExistentCarId = 9999;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => _carService.GetCarByIdAvailableByRef(nonExistentCarId));
            Assert.Contains("Car not found", ex.Message);
        }

        [Fact]
        public void GetCarByIdOnAuctionByRef_ForCarOnAuction_ShouldReturnCar()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            car.SetCarOnAuction();

            // Act
            var result = _carService.GetCarByIdOnAuctionByRef(car.Id);

            // Assert
            Assert.Equal(car, result);
        }

        [Fact]
        public void GetCarByIdOnAuctionByRef_ForAvailableCar_ShouldThrowException()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _carService.GetCarByIdOnAuctionByRef(car.Id));
            Assert.Contains("Car should be in an auction state", ex.Message);
        }

        [Fact]
        public void MarkAsAvailable_ShouldChangeCarStatus()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            car.SetCarOnAuction();

            // Act
            _carService.MarkAsAvailable(car);

            // Assert
            Assert.Equal(CarStatus.Available, car.Status);
        }

        [Fact]
        public void MarkAsSold_ShouldChangeCarStatus()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car = _carService.AddCar(CreateValidCarRequestTruck(owner.Id));
            car.SetCarOnAuction();

            // Act
            _carService.MarkAsSold(car);

            // Assert
            Assert.Equal(CarStatus.Sold, car.Status);
        }

        [Fact]
        public void SearchCars_ShouldFilterByType()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var sedan = _carService.AddCar(CreateValidCarRequestDoors(owner.Id, CarType.Sedan));
            var suv = _carService.AddCar(CreateValidCarRequestTruck(owner.Id, CarType.Truck));

            // Act
            var results = _carService.SearchCars(type: CarType.Sedan);

            // Assert
            Assert.Single(results);
            Assert.Equal(sedan.Id, results[0].Id);
        }

        [Fact]
        public void SearchCars_ShouldFilterByStatus()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var availableCar = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            var auctionCar = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            auctionCar.SetCarOnAuction();

            // Act
            var results = _carService.SearchCars(carStatus: CarStatus.OnAuction);

            // Assert
            Assert.Single(results);
            Assert.Equal(auctionCar.Id, results[0].Id);
        }

        [Fact]
        public void SearchCars_ShouldFilterByManufacturer()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var toyota = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            var hondaRequest = CreateValidCarRequestDoors(owner.Id);
            hondaRequest.Manufacturer = "Honda";
            _carService.AddCar(hondaRequest);

            // Act
            var results = _carService.SearchCars(manufacturer: "Toyota");

            // Assert
            Assert.Single(results);
            Assert.Equal(toyota.Id, results[0].Id);
        }

        [Fact]
        public void GetAllCars_ShouldReturnAllCars()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            var car1 = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));
            var car2 = _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            // Act
            var results = _carService.GetAllCars();

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Contains(car1, results);
            Assert.Contains(car2, results);
        }

        [Fact]
        public void Reset_ShouldClearInventory()
        {
            // Arrange
            var owner = _userService.AddUser(new UserRequest { Name = "Test Owner" });
            _carService.AddCar(CreateValidCarRequestDoors(owner.Id));

            // Act
            _carService.Reset();

            // Assert
            Assert.Empty(_carService.GetAllCars());
        }
    }
}
