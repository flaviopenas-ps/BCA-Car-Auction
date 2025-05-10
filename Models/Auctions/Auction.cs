using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Services;
using BCA_Car_Auction.Validation;

namespace BCA_Car_Auction.Models.Auctions
{
    public class Auction
    {
        private static int _nextId = 0;
        private readonly object _lock = new(); // Per-auction lock

        public int Id { get; init; }
        public int UserStarterId { get; private set; }
        public int CarId { get; private set; }
        private readonly LinkedList<Bid> Bids = new();
        public decimal StartBid { get; private set; }
        public decimal CurrentBid { get; private set; }
        public int? CurrentBidderId { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime StartTime { get; init; }
        public DateTime? EndTime { get; private set; }

        protected static int GetNextId() => Interlocked.Increment(ref _nextId);

        public Auction(int carId, decimal startBid, int userStarterId)
        {
            CarId = carId;
            StartBid = startBid;
            CurrentBid = startBid;
            UserStarterId = userStarterId;
            Id = GetNextId();
            IsActive = true;
            StartTime = DateTime.UtcNow;
        }

        public bool MakeBid(int userId, decimal bidAmount)
        {
            lock (_lock)
            {
                this.ThrowIfAuctionIsClosed();

                bool isIlegal = UserStarterId == userId;

                isIlegal.ThrowIfIlegalBid();

                var currentBid = Bids.FirstOrDefault();

                decimal currentBidValue = 0;

                if (currentBid == null)
                {
                    currentBidValue = CurrentBid;
                }
                else
                {
                    currentBidValue = currentBid.Amount;
                }

                bidAmount.ThrowIfBidTooLow(currentBidValue, nameof(bidAmount));

                Bids.AddFirst(new Bid(userId, bidAmount));
                CurrentBid = bidAmount;
                CurrentBidderId = userId;

                return true;
            }
        }

        public void ReOpen()
        {
            lock (_lock)
            {
                IsActive = true;
                EndTime = null;
            }
        }

        public void Close()
        {
            lock (_lock)
            {
                IsActive = false;
                EndTime = DateTime.UtcNow;
            }
        }

        private class Bid
        {
            private static int _nextId = 0;
            public int Id { get; init; }
            public int UserId { get; init; }
            public decimal Amount { get; init; }
            public DateTime Time { get; init; } = DateTime.UtcNow;

            protected static int GetNextId() => Interlocked.Increment(ref _nextId);

            public Bid(int userId, decimal amount)
            {
                Id = GetNextId();
                UserId = userId;
                Amount = amount;
            }
        }
    }
}
