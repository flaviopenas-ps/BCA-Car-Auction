using BCA_Car_Auction.Validation;
using System.ComponentModel.DataAnnotations;

namespace BCA_Car_Auction.DTOs.Auctions
{
    public class CreateAndCloseAuctionRequest
    {
        [Required]
        [PositiveNumberInt]
        public int CarId { get; set; }

        [Required]
        [PositiveNumberInt]
        public int UserId { get; set; }
    }

    public class PlaceBidRequest
    {
        [Required]
        [PositiveNumberInt]
        public int CarId { get; set; }

        [Required]
        [PositiveNumberInt]
        public int UserId { get; set; }

        [Required]
        [PositiveNumberDecimal]
        public decimal Amount { get; set; }
    }
}
