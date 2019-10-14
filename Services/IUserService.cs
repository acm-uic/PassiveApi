using PassiveApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassiveApi.Services
{
    public interface IUserService
    {
        Task<User> GetUserAsync(string userName);
    }
}
