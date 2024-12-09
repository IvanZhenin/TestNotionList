namespace Notion.DataAccess.Exceptions
{
    public class UserByIdNotFoundException(int userId)
        : Exception($"Пользователь под номером {userId} не найден!");
}