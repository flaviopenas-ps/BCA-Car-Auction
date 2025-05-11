using BCA_Car_Auction.Models.Auctions;
using BCA_Car_Auction.Models.Users;
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
        bool CloseAuction(int carId, int userId);
        
        void Reset();//tests

        Auction? GetAuction(int carId);
    }

    public class AuctionService : IAuctionService
    {
        private readonly ConcurrentDictionary<int, Auction> _auctions = new();
        
        private readonly ICarService _inventory;
        private readonly IUserService _users;

        public AuctionService(ICarService inventory, IUserService users)
        {
            _inventory = inventory;
            _users = users;
        }

        public bool CreateAuction(int carId, int userId)
        {
            try
            {
                _users.GetUserById(userId);

                _inventory.GetCarByIdAvailableByRef(carId);

                var auction = new Auction(carId, _inventory.GetStartBid(carId), userId);

                if (_inventory.GetUserId(carId) != userId)
                {
                    throw new UnauthorizedAccessException("You are not the owner of this car");
                }
                //transaction
                try
                {
                    _auctions.TryAdd(carId, auction);

                    _inventory.MarkAsOnAuction(carId);
                    return true;
                }
                catch (Exception)
                {
                    //rollback
                    if (_auctions.TryGetValue(carId, out var auction1))
                    {
                        _auctions.TryRemove(carId, out auction1);
                    }
                    _inventory.MarkAsAvailable(carId);
                    //car.SetCarAvailable();
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
                _users.GetUserById(userId);

                _auctions.TryGetValue(carId, out var auction);
                auction.ThrowIfNull("Auction not found");

                var car = _inventory.GetCarByIdOnAuctionByRef(carId);
                
                car.ThrowIfNull("Car not found");

                return auction.MakeBid(userId, amount);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CloseAuction(int carId, int userId)
        {
            try
            {
                _users.GetUserById(userId);

                _auctions.TryGetValue(carId, out var auction);
                auction.ThrowIfNull("Auction not found");

                _inventory.GetCarByIdOnAuctionByRef(carId);

                if (_inventory.GetUserId(carId) != userId)
                {
                    throw new UnauthorizedAccessException("You are not the owner of this car");
                }

                try
                {
                    _inventory.MarkAsSold(carId);

                    auction.Close(userId);
                    return true;
                }
                catch
                {
                    //rollback
                    _inventory.MarkAsOnAuction(carId);
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
        public void Reset()
        {
            _auctions.Clear();
        }
    }

}
