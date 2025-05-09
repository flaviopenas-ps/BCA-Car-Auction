using BCA_Car_Auction.Models.Auctions;
using BCA_Car_Auction.Models.Vehicles;
using System.Collections;
using System.Collections.Concurrent;

namespace BCA_Car_Auction.Services
{

    public interface IAuctionService
    {
        BidResult PlaceBid(int carId, int userId, decimal amount);
        AuctionResult CreateAuction(int carId, int userId);
        AuctionResult CloseAuction(int carId, bool wasSold);

        Auction? GetAuction(int carId);
    }

    public class AuctionService : IAuctionService
    {
        private readonly ConcurrentDictionary<int, Auction> _auctions = new();
        
        private readonly ICarService _inventory;
        private readonly ILogger<AuctionService> _logger;

        public AuctionService(ICarService inventory, ILogger<AuctionService> logger)
        {
            _inventory = inventory;
            _logger = logger;
        }

        public AuctionResult CreateAuction(int carId, int userId)
        {
            try
            {
                var car = _inventory.GetCarByIdByReference(carId);
                if (car == null)
                    return AuctionResult.CarNotFound;
                if (car.Status == CarStatus.OnAuction)
                    return AuctionResult.CarAlreadyExists;
                if (car.Status == CarStatus.Sold)
                    return AuctionResult.CarAlreadySold;

                var auction = new Auction(carId, car.StartBid, userId);
                //transaction
                try
                {
                    if (!_auctions.TryAdd(carId, auction))
                        return AuctionResult.CarAlreadyExists;

                    car.SetCarOnAuction();
                    return AuctionResult.Success;
                }
                catch (Exception ex)
                {
                    //rollback
                    if (_auctions.TryGetValue(carId, out var auction1))
                    {
                        _auctions.TryRemove(carId, out auction1);
                    }
                    car.SetCarAvailable();
                    _logger.LogError(ex, "Error creating auction for car {CarId}", carId);
                    return AuctionResult.FailOnCreation;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating auction for car {CarId}", carId);
                return AuctionResult.FailOnCreation;
            }
        }

        public BidResult PlaceBid(int carId, int userId, decimal amount)
        {
            try
            {
                if (!_auctions.TryGetValue(carId, out var auction))
                    return BidResult.AuctionNotFound;

                var car = _inventory.GetCarByIdByReference(carId);
                if (car is null)
                    return BidResult.CarNotFound;

                return auction.MakeBid(userId, amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bid on car {CarId}", carId);
                return BidResult.Failed;
            }
        }

        public AuctionResult CloseAuction(int carId, bool isSold)
        {
            try
            {
                if (!_auctions.TryGetValue(carId, out var auction))
                    return AuctionResult.AuctionNotFound;

                var car = _inventory.GetCarByIdByReference(carId);

                if (car is null)
                    return AuctionResult.CarNotFound;

                if (!(car.Status==CarStatus.OnAuction))
                    return AuctionResult.AuctionClosed;

                try
                {
                    if (isSold)
                    {
                        _inventory.MarkAsSold(car);
                    }
                    else
                    {
                        _inventory.MarkAsAvailable(car);
                    }

                    auction.Close();
                    return AuctionResult.Success;
                }
                catch
                {
                    if (isSold)
                    {
                        _inventory.MarkAsAvailable(car);
                    }
                    auction.ReOpen();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing auction for car {CarId}", carId);
                return AuctionResult.FailOnClosing;
            }
        }

        public Auction? GetAuction(int carId)
        {
            return _auctions.TryGetValue(carId, out var auction) ? auction : null;
        }
    }

}
