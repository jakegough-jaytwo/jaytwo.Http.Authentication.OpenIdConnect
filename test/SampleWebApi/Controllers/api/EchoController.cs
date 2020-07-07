using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SampleWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EchoController : ControllerBase
    {
        [HttpGet]
        [Route("echo")]
        public IActionResult Echo(string value)
        {
            return Content(value);
        }

        [Authorize]
        [HttpGet]
        [Route("userInfo")]
        public async Task<IActionResult> UserInfo()
        {             
            var authInfo = await HttpContext.AuthenticateAsync();
            //var email = authInfo.Principal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email).Value;
            return Content("ok");
        }
    }
}
