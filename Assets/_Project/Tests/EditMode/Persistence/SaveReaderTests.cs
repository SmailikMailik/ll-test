using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;
using LL.Tests.EditMode;
using LL.User.Persistence;
using LL.User.Persistence.Documents;
using NUnit.Framework;

namespace LL.Tests.EditMode.Persistence
{
    internal sealed class SaveReaderTests
    {
        [Test]
        public void LoadsWithoutWritableStorageContract()
        {
            var serializer = new JsonSaveSerializer();
            var writableStorage = new MemorySaveStorage();
            var service = new SaveService(serializer, writableStorage);
            var document = UserSaveDocumentMapper.ToDocument(TestDataFactory.CreateUserSnapshot());
            Assert.That(service.TrySave("user", document), Is.True);
            IReadOnlySaveStorage readOnlyStorage = writableStorage;
            var reader = new SaveReader(serializer, readOnlyStorage);

            var loaded = reader.TryLoad("user", out UserSaveDocument restored);

            Assert.That(loaded, Is.True);
            Assert.That(restored.Identity.UserId, Is.EqualTo("test-user"));
        }
    }
}