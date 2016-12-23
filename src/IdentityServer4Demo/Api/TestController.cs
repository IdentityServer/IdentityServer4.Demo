using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4Demo.Api
{
    [Route("api/test")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    public class TestController : ControllerBase
    {
        public IActionResult Get()
        {
            return new JsonResult(new { ok = "ok" });
        }
    }
}