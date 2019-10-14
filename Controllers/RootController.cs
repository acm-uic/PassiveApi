using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PassiveApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class RootController : ControllerBase
    {
        private readonly ILogger<RootController> _logger;
        public RootController(ILogger<RootController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var response = new
            {
                href = Url.Link(nameof(GetRoot), null),
                users = new
                {
                    href = Url.Link(
                        nameof(UsersController.GetUsers), null)
                }
            };

            return Ok(response);
        }
    }
}
