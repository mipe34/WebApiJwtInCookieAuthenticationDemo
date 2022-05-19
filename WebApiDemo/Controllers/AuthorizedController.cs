using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizedController : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult AuthorizedAction()
        {
            return Ok(new { authorized = "true"});
        }
    }
}
