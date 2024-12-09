using Microsoft.EntityFrameworkCore;
using Notion.BaseModule.Interfaces;
using Notion.DataAccess.Data;
using Notion.DataAccess.Exceptions;
using Notion.DataAccess.Models;
using System.Linq;

namespace Notion.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly ICacheService _cache;
        private readonly NotionDb _context;
        private readonly IPasswordHasher _passwordHasher;
        private const string USER_KEY = "User";
        public UserService(NotionDb context, 
            ICacheService cache, 
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _cache = cache;
            _passwordHasher = passwordHasher;
        }
        public async Task<User> CreateUserAsync(string login, string password)
        {
            try
            {
                var userExists = await _context.Users.AsNoTracking().AnyAsync(u => u.Login == login);
                if (userExists)
                    throw new UserLoginIsUnavailableException(login);

                var newUser = new User
                {
                    Login = login,
                    PasswordHash = _passwordHasher.Generate(password),
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            if (!_cache.TryGet(USER_KEY + userId, out User? user))
            {
                user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                await _cache.SetAsync(USER_KEY + userId, user, TimeSpan.FromMinutes(1));
            }

            return user ?? throw new UserByIdNotFoundException(userId);
        }

        public async Task<User> GetUserByLoginAsync(string userLogin)
        {
            if (!_cache.TryGet(USER_KEY + userLogin, out User? user))
            {
                user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login == userLogin);
                await _cache.SetAsync(USER_KEY + userLogin, user, TimeSpan.FromMinutes(1));
            }

            return user ?? throw new UserByLoginNotFoundException(userLogin);
        }

        public async Task<bool> CheckUserDataAsync(string userLogin, string userPassword)
        {
            var user = await GetUserByLoginAsync(userLogin);

            return user.Login == userLogin && _passwordHasher.CheckPassword(userPassword, user.PasswordHash);
        }
    }
}