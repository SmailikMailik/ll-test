using LL.User.Snapshots;

namespace LL.User.Persistence
{
    internal interface IUserSaveRepository
    {
        UserLoadResult Load();
        bool Save(UserSnapshot snapshot);
        bool Exists();
        bool Delete();
    }
}