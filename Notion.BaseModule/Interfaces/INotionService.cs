using Notion.DataAccess.Models;
using System.Collections.Generic;

namespace Notion.BaseModule.Interfaces
{
    public interface INotionService
    {
        public Task<IEnumerable<UserNotion>> GetNotionListByUserLoginAsync(string userLogin);
        public Task<UserNotion> AddNotionAsync(string userLogin, string text);
        public Task<UserNotion> ChangeNotionAsync(Guid notionId, string? newText, bool? isComplited);
        public Task DeleteNotionAsync(Guid notionId);
    }
}