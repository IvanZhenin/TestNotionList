namespace Notion.DataAccess.Exceptions
{
    public class UserLoginIsUnavailableException(string userLogin)
        : Exception($"Пользователь с логином {userLogin} уже существует!");
}