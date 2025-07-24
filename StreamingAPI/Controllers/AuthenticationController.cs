using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamingApplication.Commands.Authentication;
using StreamingApplication.Interfaces.Commands.Authentication;

namespace StreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILoginCommandHandler _loginCommandHandler;
        public AuthenticationController(ILoginCommandHandler loginCommandHandler)
        {
            _loginCommandHandler = loginCommandHandler;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> UserLogin([FromBody] LoginCommand loginCommand)
        {
            try
            {
                var result = await _loginCommandHandler.UserLogin(loginCommand);

                // Set refresh token in cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(30)
                };

                Response.Cookies.Append("refreshToken", result.Data.LastOrDefault(), cookieOptions);

                if(result.IsSuccess == false)
                {
                    if(result.ErrorCode == "400")
                    {
                        return BadRequest(result.Error);
                    }
                    else
                    {
                        return Unauthorized(result.Error);
                    }
                }

                return Ok(result.Data.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
