using BCA_Car_Auction.DTOs;
using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Services;
using BCA_Car_Auction.Validation;
using Microsoft.AspNetCore.Mvc;

namespace BCA_Car_Auction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _inventory;

        public CarsController(ICarService inventory)
        {
            _inventory = inventory;
        }

        [HttpPost("add")]
        public IActionResult AddCar([FromBody] CarRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var car = _inventory.AddCar(request);
                return Ok(car);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        public IActionResult GetAllCars() => Ok(_inventory.GetAllCars());

        [HttpGet("available")]
        public IActionResult GetAvailableCars() => Ok(_inventory.SearchCars(carStatus: CarStatus.Available));

        [HttpGet("get")]
        public IActionResult GetCarsByModel([FromQuery] string model)
        {
            var car = Ok(_inventory.SearchCars(model: model));
            return car is null ? NotFound() : Ok(car);
        }
    }
}
