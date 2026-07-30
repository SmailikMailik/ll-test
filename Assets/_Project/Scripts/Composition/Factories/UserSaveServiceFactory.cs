using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;

namespace LL.Composition.Factories
{
    internal static class UserSaveServiceFactory
    {
        internal static ISaveService CreateJsonFile()
        {
            var serializer = new JsonSaveSerializer();
            var storage = new FileSaveStorage();

            return new SaveService(serializer, storage);
        }
    }
}