using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        //[HttpPost]
        //[Route("test")]
        //public async Task<IActionResult> Get([FromBody] string email)
        //{
        //    try
        //    {
        //        var result = await _loginCommandHandler.Get(email);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
