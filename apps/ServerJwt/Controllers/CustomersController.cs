using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ServerJwt.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CustomersController : ControllerBase
  {
    [HttpGet, Authorize(Roles = "Manager")]
    public IEnumerable<string> Get()
    {
      return new string[] { "John Doe", "Jane Doe" };
    }
  }
}
