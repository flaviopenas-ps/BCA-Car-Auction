using BCA_Car_Auction.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace BCA_Car_Auction.DTOs.Users
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        

        public static UserResponse FromUser(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name
            };
        }
    }
}
