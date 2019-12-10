using PassiveApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassiveApi.Services
{
    public class DefaultUserService : IUserService
    {
        public Task<User> GetUserAsync(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
