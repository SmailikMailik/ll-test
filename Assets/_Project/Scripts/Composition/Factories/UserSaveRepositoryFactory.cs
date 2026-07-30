using LL.User.Persistence;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;

namespace LL.Composition.Factories
{
    internal static class UserSaveRepositoryFactory
    {
        internal static IUserSaveRepository CreateJsonFile()
        {
            var serializer = new JsonSaveSerializer();
            var storage = new FileSaveStorage();

            return new SerializedUserSaveRepository(new SaveService(serializer, storage));
        }
    }
}