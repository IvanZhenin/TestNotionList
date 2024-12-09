namespace Notion.DataAccess.Models
{
    public class User
    {
        public int Id { get; private init; }
        public required string Login { get; set; }
        public required string PasswordHash { get; set; }
        public ICollection<UserNotion> UserNotions { get; set; } = [];
    }
}