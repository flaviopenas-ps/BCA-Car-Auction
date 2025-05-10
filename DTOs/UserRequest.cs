using BCA_Car_Auction.Validation;
using System.ComponentModel.DataAnnotations;

namespace BCA_Car_Auction.DTOs
{
    public class UserRequest
    {
        [Required]
        [NameValidator]
        public string Name { get; init; } = string.Empty;
    }
}
