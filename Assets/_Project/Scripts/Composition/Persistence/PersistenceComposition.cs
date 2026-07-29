using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;

namespace LL.Composition.Persistence
{
    internal static class PersistenceComposition
    {
        internal static ISaveService CreateDefaultSaveService()
        {
            var serializer = new JsonSaveSerializer();
            var storage = new FileSaveStorage();

            return new SaveService(serializer, storage);
        }
    }
}