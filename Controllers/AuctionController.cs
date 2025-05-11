using Microsoft.AspNetCore.Mvc;
using BCA_Car_Auction.Services;
using BCA_Car_Auction.DTOs.Auctions;

namespace BCA_Car_Auction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpPost("create")]
        public IActionResult CreateAuction([FromBody] CreateAndCloseAuctionRequest request)
        {
            var result = _auctionService.CreateAuction(request.CarId, request.UserId);
            return result ? Ok("Auction created successfully") : BadRequest("Auction could not be created");

        }

        [HttpPost("bid")]
        public IActionResult PlaceBid([FromBody] PlaceBidRequest request)
        {
            var result = _auctionService.PlaceBid(request.CarId, request.UserId, request.Amount);
                return result ? Ok("Bid placed successfully") : BadRequest("Failed to place bid");
        }

        [HttpPost("close")]
        public IActionResult CloseAuction([FromBody] CreateAndCloseAuctionRequest request)
        {
            try
            {
                var result = _auctionService.CloseAuction(request.CarId, request.UserId);
                return result ? Ok("Auction closed successfully") : BadRequest("Failed to close auction");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
