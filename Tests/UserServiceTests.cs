using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Services;

namespace BCA_Car_Auction.Tests
{
    using Xunit;
    using BCA_Car_Auction.Services;
    using BCA_Car_Auction.Models.Vehicles;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using BCA_Car_Auction.Models.Users;
    using BCA_Car_Auction.DTOs;

    public class UserServiceFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }

        public UserServiceFixture()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IUserService, UserService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            // Clean up if needed
        }
    }

    public class UserServiceTests : IClassFixture<UserServiceFixture>
    {
        private readonly IUserService _userService;

        public UserServiceTests(UserServiceFixture fixture)
        {
            _userService = fixture.ServiceProvider.GetRequiredService<IUserService>();
            _userService.Reset(); // Clean slate for each test
        }

        [Fact]
        public void AddUser_ShouldAddUserAndReturnIt()
        {
            // Arrange
            var request = new UserRequest { Name = "Test User" };

            // Act
            var result = _userService.AddUser(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test User", result.Name);
            Assert.NotEqual(0, result.Id);

            // Verify the user exists in the collection
            var retrievedUser = _userService.GetUserById(result.Id);
            Assert.Equal(result, retrievedUser);
        }

        [Fact]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var user1 = _userService.AddUser(new UserRequest { Name = "User 1" });
            var user2 = _userService.AddUser(new UserRequest { Name = "User 2" });

            // Act
            var result = _userService.GetAllUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(user1, result);
            Assert.Contains(user2, result);
        }

        [Fact]
        public void GetUserById_ShouldThrowWhenUserNotFound()
        {
            // Arrange
            var nonExistentId = 9999;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _userService.GetUserById(nonExistentId));
            Assert.Contains("No users found with that id", exception.Message);
        }
    }
}
