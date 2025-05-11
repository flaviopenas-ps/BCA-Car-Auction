namespace BCA_Car_Auction.DTOs.Auctions
{
    public class AuctionResponse
    {
        public int CarId { get; set; }
        public decimal CurrentBid { get; set; }
        public int OwnerId { get; set; }
        public bool IsOpen { get; set; }

        public static AuctionResponse FromAuction(BCA_Car_Auction.Models.Auctions.Auction auction)
        {
            return new AuctionResponse
            {
                CarId = auction.CarId,
                CurrentBid = auction.CurrentBid,
                OwnerId = auction.UserStarterId,
                IsOpen = auction.IsActive
            };
        }
    }
}
