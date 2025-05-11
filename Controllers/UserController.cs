using BCA_Car_Auction.DTOs.Users;
using BCA_Car_Auction.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{

    private readonly IUserService _userService;

    public UsersController(IUserService userService) // MUST use IUserService here
    {
        _userService = userService;
    }

    // POST api/users
    [HttpPost]
    public ActionResult<UserResponse> AddUser([FromBody] UserRequest request)
    {
        var user = _userService.AddUser(request);
        return Ok(UserResponse.FromUser(user));
    }

    // GET api/users
    [HttpGet]
    public ActionResult<IEnumerable<UserResponse>> GetAllUsers()
    {
        var users = _userService.GetAllUsers();
        var response = users.Select(UserResponse.FromUser);
        return Ok(response);
    }

    // GET api/users/name/{name}
    [HttpGet("name/{name}")]
    public ActionResult<UserResponse> GetUserByName(string name)
    {
        var user = _userService.GetUserByName(name);
        return Ok(UserResponse.FromUser(user));
    }

    // GET api/users/{userId}
    [HttpGet("{userId}")]
    public ActionResult<UserResponse> GetUserById(int userId)
    {
         var user = _userService.GetUserById(userId);
         return Ok(UserResponse.FromUser(user));
    }
}