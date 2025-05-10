using BCA_Car_Auction.DTOs;
using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Services;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;

namespace BCA_Car_Auction.Tests
{
    public class AuctionServiceFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }

        public AuctionServiceFixture()
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
    public class AuctionServiceTests : IClassFixture<AuctionServiceFixture>
    {
        private readonly IAuctionService _auctionService;
        private readonly ICarService _carService;
        private readonly IUserService _userService;

        public AuctionServiceTests(AuctionServiceFixture fixture)
        {
            _auctionService = fixture.ServiceProvider.GetRequiredService<IAuctionService>();
            _carService = fixture.ServiceProvider.GetRequiredService<ICarService>();
            _userService = fixture.ServiceProvider.GetRequiredService<IUserService>();

            // Clear state before each test
            ResetTestState();
        }

        private void ResetTestState()
        {
            // Clear all data before each test
            if (_userService is UserService userService)
            {
                userService.Reset();
            }
            if (_carService is CarService carService)
            {
                carService.Reset();
            }
            if (_auctionService is AuctionService auctionService)
            {
                auctionService.Reset();
            }
        }

        private (Car testCar, int ownerId, int otherUserId) SetupTestData()
        {
            var ownerId = _userService.AddUser(new UserRequest { Name = "Owner" }).Id;
            var otherUserId = _userService.AddUser(new UserRequest { Name = "Other User" }).Id;

            var carRequest = new CarRequest
            {
                Manufacturer = "Test",
                Model = "Model",
                Year = 2023,
                StartBid = 10000,
                UserIdOwner = ownerId,
                Type = CarType.Sedan,
                NumberOfDoors = 4
            };

            var testCar = _carService.AddCar(carRequest);
            return (testCar, ownerId, otherUserId);
        }

        #region Auction State Tests

        [Fact]
        public void CloseAuction_ShouldMarkCarAsSold()
        {
            // Arrange
            var (testCar, ownerId, _) = SetupTestData();
            _auctionService.CreateAuction(testCar.Id, ownerId);

            // Act
            var result = _auctionService.CloseAuction(testCar.Id, ownerId);

            // Assert
            Assert.True(result);
            Assert.Equal(CarStatus.Sold, testCar.Status);
        }

        [Fact]
        public void CloseAuction_WhenAlreadyClosed_ShouldReturnFalse()
        {
            // Arrange
            var (testCar, ownerId, _) = SetupTestData();
            _auctionService.CreateAuction(testCar.Id, ownerId);
            _auctionService.CloseAuction(testCar.Id, ownerId);

            var exception = Assert.Throws<Exception>(() =>
            _auctionService.CloseAuction(testCar.Id, ownerId));

            Assert.Contains("Car should be in an auction state.", exception.Message);
        }

        [Fact]
        public void CloseAuction_ByNonOwner_ShouldThrowUnauthorizedAccess()
        {
            // Arrange
            var (testCar, ownerId, otherUserId) = SetupTestData();
            _auctionService.CreateAuction(testCar.Id, ownerId);

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() =>
                _auctionService.CloseAuction(testCar.Id, otherUserId));
        }

        [Fact]
        public void CreateAuction_ForSoldCar_ShouldThrowInvalidOperation()
        {
            // Arrange
            var (testCar, ownerId, _) = SetupTestData();
            testCar.SetCarSold();

            var exception = Assert.Throws<Exception>(() =>
            _auctionService.CreateAuction(testCar.Id, ownerId));

            Assert.Contains("Car should be in an auction state", exception.Message);
        }

        [Fact]
        public void CreateAuction_ForNonExistentCar_ShouldThrowException()
        {
            // Arrange
            var (_, ownerId, _) = SetupTestData();
            var nonExistentCarId = 9999;

            var exception = Assert.Throws<ArgumentNullException>(() =>
            _auctionService.CreateAuction(nonExistentCarId, ownerId));

            Assert.Contains("Car not found", exception.Message);
        }

        #endregion

        #region Bid Tests

        [Fact]
        public void PlaceBid_BelowCurrentBid_ShouldThrowException()
        {
            // Arrange
            var (testCar, ownerId, otherUserId) = SetupTestData();
            _auctionService.CreateAuction(testCar.Id, ownerId);

            // Act & Assert
            Assert.Throws<Exception>(() =>
                _auctionService.PlaceBid(testCar.Id, otherUserId, 9999));
        }

        [Fact]
        public void PlaceBid_ByOwner_ShouldThrowException()
        {
            // Arrange
            var (testCar, ownerId, _) = SetupTestData();
            _auctionService.CreateAuction(testCar.Id, ownerId);

            // Act & Assert
            Assert.Throws<Exception>(() =>
                _auctionService.PlaceBid(testCar.Id, ownerId, 11000));
        }

        [Fact]
        public void PlaceBid_OnNonExistentAuction_ShouldThrowException()
        {
            // Arrange
            var (testCar, ownerId, otherUserId) = SetupTestData();
            var nonExistentCarId = 9999;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _auctionService.PlaceBid(nonExistentCarId, otherUserId, 11000));
        }

        [Fact]
        public void PlaceBid_OnClosedAuction_ShouldThrowException()
        {
            // Arrange
            var (testCar, ownerId, otherUserId) = SetupTestData();
            _auctionService.CreateAuction(testCar.Id, ownerId);
            _auctionService.CloseAuction(testCar.Id, ownerId);

            // Act & Assert
            Assert.Throws<Exception>(() =>
                _auctionService.PlaceBid(testCar.Id, otherUserId, 11000));
        }

        [Fact]
        public void PlaceBid_InvalidBid_ShouldThrowAnError()
        {
            // Arrange
            var (testCar, ownerId, otherUserId) = SetupTestData();
            _auctionService.CreateAuction(testCar.Id, ownerId);

            // Act
            _auctionService.PlaceBid(testCar.Id, otherUserId, 11000);
            _auctionService.PlaceBid(testCar.Id, otherUserId, 12000);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _auctionService.PlaceBid(testCar.Id, otherUserId, 11000));
        }

        #endregion
    }
}
