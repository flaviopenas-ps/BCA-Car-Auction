using BCA_Car_Auction.Models.Auctions;
using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Validation;
using System.Collections;
using System.Collections.Concurrent;

namespace BCA_Car_Auction.Services
{

    public interface IAuctionService
    {
        bool PlaceBid(int carId, int userId, decimal amount);
        bool CreateAuction(int carId, int userId);
        bool CloseAuction(int carId,int userId, bool wasSold);

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

        public bool CreateAuction(int carId, int userId)
        {
            try
            {
                var car = _inventory.GetCarByIdAvailableByRef(carId);

                var auction = new Auction(carId, car.StartBid, userId);

                if (car.UserIdOwner != userId)
                {
                    throw new UnauthorizedAccessException("You are not the owner of this car");
                }
                //transaction
                try
                {
                    if (!_auctions.TryAdd(carId, auction))
                        car.ThrowIfCarAlreadyInAuction();

                    car.SetCarOnAuction();
                    return true;
                }
                catch (Exception)
                {
                    //rollback
                    if (_auctions.TryGetValue(carId, out var auction1))
                    {
                        _auctions.TryRemove(carId, out auction1);
                    }
                    car.SetCarAvailable();
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool PlaceBid(int carId, int userId, decimal amount)
        {
            try
            {
                _auctions.TryGetValue(carId, out var auction);
                auction.ThrowIfNull("Auction not found");

                var car = _inventory.GetCarByIdAvailableByRef(carId);
                
                car.ThrowIfNull("Car not found");

                return auction.MakeBid(userId, amount);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CloseAuction(int carId, int userId, bool isSold)
        {
            try
            {
                _auctions.TryGetValue(carId, out var auction);
                auction.ThrowIfNull("Auction not found");

                var car = _inventory.GetCarByIdAvailableByRef(carId);

                car.ThrowIfNull("Car not found");

                car.ThrowIfCarNotOnAuction();

                if (car.UserIdOwner != userId)
                {
                    throw new UnauthorizedAccessException("You are not the owner of this car");
                }

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

                    auction.Close(userId);
                    return true;
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
            catch (Exception)
            {
                throw;
            }
        }



        public Auction? GetAuction(int carId)
        {
            return _auctions.TryGetValue(carId, out var auction) ? auction : null;
        }
    }

}
