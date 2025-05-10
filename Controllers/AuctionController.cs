using BCA_Car_Auction.DTOs;
using BCA_Car_Auction.Models.Auctions;
using BCA_Car_Auction.Services;
using Microsoft.AspNetCore.Mvc;

namespace BCA_Car_Auction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;
        private readonly ILogger<AuctionController> _logger;

        public AuctionController(IAuctionService auctionService, ILogger<AuctionController> logger)
        {
            _auctionService = auctionService;
            _logger = logger;
        }

        //[HttpPost("create/{carId}")]
        //public IActionResult CreateAuction(int carId, [FromQuery] int userId)
        //{
        //    var result = _auctionService.CreateAuction(carId, userId);

        //    return result switch
        //    {
        //        AuctionResult.CarNotFound => NotFound("Vehicle with id not found."),
        //        AuctionResult.Success => Ok("Auction created successfully."),
        //        _ => BadRequest("Auction could not be created.")
        //    };
        //}

        //[HttpPost("place-bid/{carId}")]
        //public IActionResult PlaceBid([FromBody] PlaceBidRequest request)
        //{
        //    var result = _auctionService.PlaceBid(request.CarId, request.UserId, request.Amount);

        //    return result switch
        //    {
        //        BidResult.Success => Ok(new { message = "Bid placed successfully." }),
        //        BidResult.BidTooLow => BadRequest(new { message = "Bid is too low." }),
        //        BidResult.CarNotFound => NotFound(new { message = "Car not found." }),
        //        BidResult.AuctionNotFound => NotFound(new { message = "Auction not found." }),
        //        BidResult.AlreadySold => BadRequest(new { message = "Car already sold." }),
        //        BidResult.IlegalBid => BadRequest(new { message = "You cannot bid on your own auction." }),
        //        _ => StatusCode(500, new { message = "Unknown error placing bid." })
        //    };
        //}


        [HttpGet("{carId}")]
        public IActionResult GetAuction(int carId)
        {
            var auction = _auctionService.GetAuction(carId);
            return auction is null ? NotFound("No auction found.") : Ok(auction);
        }
    }
}
