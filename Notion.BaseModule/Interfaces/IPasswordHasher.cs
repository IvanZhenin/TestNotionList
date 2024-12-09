namespace Notion.BaseModule.Interfaces
{
    public interface IPasswordHasher
    {
        public string Generate(string password);
        public bool CheckPassword(string password, string hashedPassword);
    }
}