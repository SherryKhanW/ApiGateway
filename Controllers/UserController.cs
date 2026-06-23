using Microsoft.AspNetCore.Mvc;
using UserManagement.Grpc.Contracts;

namespace ApiGateway.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserGrpcService _userGrpcService;

    public UsersController(IUserGrpcService userGrpcService)
    {
        _userGrpcService = userGrpcService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUser([FromQuery] string email)
    {
        var response = await _userGrpcService.GetUserAsync(
            new GetUserRequest
            {
                Email = email
            });

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}