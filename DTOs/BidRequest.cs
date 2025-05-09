namespace BCA_Car_Auction.DTOs
{
    public class PlaceBidRequest
    {
        public int CarId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
