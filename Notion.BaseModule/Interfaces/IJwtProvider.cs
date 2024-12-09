namespace Notion.BaseModule.Interfaces
{
    public interface IJwtProvider
    {
        public string GenerateToken(int userId, string userLogin);
    }
}