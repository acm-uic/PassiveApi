using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PassiveApi.Services;
using PassiveApi.Models;

namespace PassiveApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;

        }

        // GET /user/{userName}
        [HttpGet("{userName}", Name = nameof(GetUserByUserName))]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<User>> GetUserByUserName(string userName)
        {
            var user = await _userService.GetUserAsync(userName);
            if (user == null) return NotFound();

            return user;
        }

        public Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
