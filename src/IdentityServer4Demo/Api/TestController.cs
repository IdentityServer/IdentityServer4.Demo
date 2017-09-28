//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;

//namespace IdentityServer4Demo.Api
//{
//    [Route("/api/test")]
//    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
//    public class TestController : ControllerBase
//    {
//        public IActionResult Get()
//        {
//            var claims = User.Claims.Select(c => new { c.Type, c.Value });
//            return new JsonResult(claims);
//        }
//    }
//}