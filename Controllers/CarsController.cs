using Microsoft.AspNetCore.Mvc;
using BCA_Car_Auction.Services;
using BCA_Car_Auction.DTOs;
using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.DTOs.Cars;
using BCA_Car_Auction.DTOs.Auctions;

namespace BCA_Car_Auction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpPost]
        public ActionResult<CarResponse> CreateCar([FromBody] CarRequest request)
        {
            var car = _carService.AddCar(request);
            return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, CarResponse.FromCar(car));
        }

        [HttpGet("search")]
        public ActionResult<List<CarResponse>> SearchCars([FromQuery] CarRequestSearch request)
        {
            var results = _carService.SearchCars(request.Type, request.Status, request.Manufacturer, request.Model, request.Year);
            return results.Select(CarResponse.FromCar).ToList();
        }

        [HttpGet("{carId}")]
        public ActionResult<AuctionResponse> GetAuction(int carId)
        {
            var auction = _auctionService.GetAuction(carId);
            if (auction == null)
                return NotFound("Auction not found");

            return AuctionResponse.FromAuction(auction);
        }
    }
}