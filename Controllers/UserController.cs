
using BCA_Car_Auction.DTOs;
using BCA_Car_Auction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BCA_User_Auction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _users;

        public UserController(IUserService users)
        {
            _users = users;
        }

        [HttpPost("add")]
        public IActionResult AddUser([FromBody] UserRequest request)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpGet("update")]
        public IActionResult GetAvailableUsers()
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }
    }
}
