using BCA_Car_Auction.DTOs;
using BCA_Car_Auction.Models.Auctions;
using BCA_Car_Auction.Models.Users;
using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Validation;
using System.Collections.Concurrent;

namespace BCA_Car_Auction.Services
{
    public interface IUserService
    {
        User AddUser(UserRequest request);
        List<User> GetAllUsers();
        User GetUserById(int userId);
        User GetUserByName(string name);
    }
    public class UserService : IUserService
    {
        private readonly ConcurrentDictionary<int, User> _users = new();
        private readonly IAuctionService _auctionService;
        public UserService(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }
        public User AddUser(UserRequest request)
        {
            var user = new User(request.Name);
            _users.TryAdd(user.Id, user);
            return user;
        }
        public List<User> GetAllUsers()
        {
            return _users.Values.ToList();
        }
        public User GetUserByName(string name)
        {
            return _users.Values.FirstOrDefault(u => u.Name != null && u.Name.
            Contains(name, StringComparison.OrdinalIgnoreCase))
                .ThrowIfNull("No users found with that name");
        }
        public User GetUserById(int userId)
        {
            return _users[userId].ThrowIfNull("No users found with that id");
        }
    }
}
