using Notion.DataAccess.Models;
using System.Collections.Generic;

namespace Notion.BaseModule.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetUserByIdAsync(int userId);
        public Task<User> GetUserByLoginAsync(string login);
        public Task<bool> CheckUserDataAsync(string login, string password);
        public Task<User> CreateUserAsync(string login, string password);
    }
}