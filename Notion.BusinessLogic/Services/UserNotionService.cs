using Microsoft.EntityFrameworkCore;
using Notion.BaseModule.Interfaces;
using Notion.DataAccess.Data;
using Notion.DataAccess.Exceptions;
using Notion.DataAccess.Models;
using System.Linq;

namespace Notion.BusinessLogic.Services
{
    public class UserNotionService : INotionService
    {
        private readonly ICacheService _cache;
        private readonly NotionDb _context;
        private const string NOTION_LIST_KEY = "Notions";
        private const string NOTION_KEY = "Notion";
        public UserNotionService(NotionDb context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<UserNotion> AddNotionAsync(string userLogin, string text)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == userLogin)
                    ?? throw new UserByLoginNotFoundException(userLogin);

                var newNotion = new UserNotion
                {
                    Text = text,
                    UserId = user.Id,
                    User = user,
                };

                _context.UserNotions.Add(newNotion);
                await _context.SaveChangesAsync();
                await ClearCache(NOTION_KEY + newNotion.Id, 
                    NOTION_LIST_KEY + newNotion.UserId);

                return newNotion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserNotion> ChangeNotionAsync(Guid notionId, string? newText, bool? isComplited)
        {
            try
            {
                var notion = await _context.UserNotions.FirstOrDefaultAsync(u => u.Id == notionId)
                    ?? throw new UserNotionNotFoundException();

                notion.IsComplited = isComplited ?? notion.IsComplited;
                notion.Text = newText ?? notion.Text;

                await _context.SaveChangesAsync();
                await ClearCache(NOTION_KEY + notionId, 
                    NOTION_LIST_KEY + notion.UserId);

                return notion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteNotionAsync(Guid notionId)
        {
            try
            {
                var notion = await _context.UserNotions.FirstOrDefaultAsync(u => u.Id == notionId)
                    ?? throw new Exception();

                _context.UserNotions.Remove(notion);
                await _context.SaveChangesAsync();
                await ClearCache(NOTION_KEY + notionId, 
                    NOTION_LIST_KEY + notion.UserId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<UserNotion>> GetNotionListByUserLoginAsync(string userLogin)
        {
            if (!_cache.TryGet(NOTION_LIST_KEY + userLogin, out List<UserNotion>? notions) 
                || notions == null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == userLogin)
                    ?? throw new UserByLoginNotFoundException(userLogin);

                notions = await _context.UserNotions.AsNoTracking().Where(n => n.UserId == user.Id).ToListAsync();
                await _cache.SetAsync(NOTION_LIST_KEY + userLogin, notions, TimeSpan.FromMinutes(1));
            }

            return notions;
        }

        private async Task ClearCache(params string[] keys)
        {
            foreach (var key in keys)
            {
                await _cache.RemoveAsync(key);
            }
        }
    }
}