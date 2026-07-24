namespace LL.User
{
    internal interface IUserDataSource
    {
        UserDataSnapshot Load();
    }
}