using BCA_Car_Auction.Models.Auctions;

namespace BCA_Car_Auction.DTOs.Auctions
{
    public class BidResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Time { get; set; }
    }

    public class AuctionResponse
    {
        public int CarId { get; set; }
        public decimal CurrentBid { get; set; }
        public int OwnerId { get; set; }
        public bool IsOpen { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<BidResponse> Bids { get; set; } = new();

        public static AuctionResponse FromAuction(Auction auction)
        {
            return new AuctionResponse
            {
                CarId = auction.CarId,
                CurrentBid = auction.CurrentBid,
                OwnerId = auction.UserStarterId,
                IsOpen = auction.IsActive,
                StartTime = auction.StartTime,
                EndTime = auction.EndTime,
                Bids = auction.GetBids().Select(b => new BidResponse
                {
                    Id = (int)b.GetType().GetProperty("Id")!.GetValue(b)!,
                    UserId = (int)b.GetType().GetProperty("UserId")!.GetValue(b)!,
                    Amount = (decimal)b.GetType().GetProperty("Amount")!.GetValue(b)!,
                    Time = (DateTime)b.GetType().GetProperty("Time")!.GetValue(b)!
                }).ToList()
            };
        }
    }
}
