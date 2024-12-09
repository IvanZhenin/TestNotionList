using Notion.DataAccess.Models;
using System.Collections.Generic;

namespace Notion.BaseModule.Interfaces
{
    public interface INotionService
    {
        public Task<IEnumerable<UserNotion>> GetNotionListByUserIdAsync(int userId);
        public Task<UserNotion> AddNotionAsync(int userId, string text);
        public Task<UserNotion> ChangeNotionAsync(Guid notionId, string? newText, bool? isComplited);
        public Task DeleteNotionAsync(Guid notionId);
    }
}