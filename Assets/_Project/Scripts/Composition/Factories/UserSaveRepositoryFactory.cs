using LL.User.Persistence;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;
using LL.Infrastructure.Saving;

namespace LL.Composition.Factories
{
    internal static class UserSaveRepositoryFactory
    {
        internal static IUserSaveRepository CreateSerialized(
            ISaveSerializer serializer,
            ISaveStorage storage)
        {
            return new SerializedUserSaveRepository(new SaveService(serializer, storage));
        }
    }
}