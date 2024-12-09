namespace Notion.DataAccess.Models
{
    public class UserNotion
    {
        public Guid Id { get; private init; } = Guid.NewGuid();
        public DateTime DateCreate { get; private init; } = DateTime.UtcNow;
        public bool IsComplited { get; set; } = false;
        public required string Text { get; set; }
        public required int UserId { get; set; }
        public required User User { get; set; }
    }
}