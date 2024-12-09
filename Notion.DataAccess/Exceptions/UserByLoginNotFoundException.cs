namespace Notion.DataAccess.Exceptions
{
    public class UserByLoginNotFoundException(string userLogin) 
        : Exception($"Пользователь с логином {userLogin} не найден!");
}