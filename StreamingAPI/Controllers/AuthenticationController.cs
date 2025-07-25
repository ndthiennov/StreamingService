using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamingApplication.Commands.Authentication;
using StreamingApplication.Commands.Handlers.Authentication;
using StreamingApplication.Interfaces.Commands.Authentication;

namespace StreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly ILoginCommandHandler _loginCommandHandler;
        private readonly ISubmitUserEmailHandler _submitUserEmailHandler;
        private readonly IInviteUserCommandHandler _inviteUserCommandHandler;
        private readonly ISendOtpCommandHandler _sendOtpCommandHandler;
        private readonly IForgotPasswordCommandHandler _forgotPasswordCommandHandler;
        public AuthenticationController(ILogger<AuthenticationController> logger,
            ILoginCommandHandler loginCommandHandler, 
            ISubmitUserEmailHandler submitUserEmailHandler,
            IInviteUserCommandHandler inviteUserCommandHandler,
            ISendOtpCommandHandler sendOtpCommandHandler,
            IForgotPasswordCommandHandler forgotPasswordCommandHandler)
        {
            _logger = logger;
            _loginCommandHandler = loginCommandHandler;
            _submitUserEmailHandler = submitUserEmailHandler;
            _inviteUserCommandHandler = inviteUserCommandHandler;
            _sendOtpCommandHandler = sendOtpCommandHandler;
            _forgotPasswordCommandHandler = forgotPasswordCommandHandler;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> UserLogin([FromBody] LoginCommand loginCommand)
        {
            try
            {
                var result = await _loginCommandHandler.Login(loginCommand);

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
                _logger.LogError(ex.Message, "Presentation-Controllers-UserLogin");
                //Console.WriteLine($"Presentation-Controllers-UserLogin: {ex.Message}");
                return BadRequest("Request failed");
            }
        }
        [HttpPost]
        [Route("submitemail")]
        public async Task<IActionResult> SubmitUserEmail([FromBody] string email)
        {
            try
            {
                var result = await _submitUserEmailHandler.SubmitUserEmail(email);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Presentation-Controllers-SubmitUserEmail");
                //Console.WriteLine($"Presentation-Controllers-SubmitUserEmail: {ex.Message}");
                return BadRequest("Request failed");
            }
        }

        [HttpPost]
        [Route("inviteuser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> InviteUser([FromBody] string email)
        {
            try
            {
                var result = await _inviteUserCommandHandler.InviteUser(email);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Presentation-Controllers-InviteUser");
                //Console.WriteLine($"Presentation-Controllers-InviteUser: {ex.Message}");
                return BadRequest("Request failed");
            }
        }
        [HttpPost]
        [Route("sendotp")]
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            try
            {
                var result = await _sendOtpCommandHandler.SendOtp(email);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Presentation-Controllers-SendOtp");
                //Console.WriteLine($"Presentation-Controllers-SendOtp: {ex.Message}");
                return BadRequest("Request failed");
            }
        }

        [HttpPost]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand forgotPasswordCommand)
        {
            try
            {
                var result = await _forgotPasswordCommandHandler.ForgotPassword(forgotPasswordCommand);

                if(result.IsSuccess == false)
                {
                    return BadRequest(result.Error);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Presentation-Controllers-ForgotPassword");
                //Console.WriteLine($"Presentation-Controllers-ForgotPassword: {ex.Message}");
                return BadRequest("Request failed");
            }
        }
    }
}
