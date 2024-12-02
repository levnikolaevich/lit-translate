using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SDT.LApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        public VersionController()
        {

        }

        [HttpGet("get-version", Name = "GetVersion")]
        public string GetVersion()
        {
            var buildDate = Environment.GetEnvironmentVariable("APP_VERSION");
            return string.IsNullOrEmpty(buildDate) ?
                "Version information not available" :
                buildDate;
        }
    }
}
