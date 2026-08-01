using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;
using LL.Tests.EditMode.Infrastructure.Saving.Storage;
using LL.Tests.EditMode.TestData;
using LL.User.Persistence;
using LL.User.Persistence.Documents;
using NUnit.Framework;

namespace LL.Tests.EditMode.Infrastructure.Saving
{
    internal sealed class SaveReaderTests
    {
        private const string SaveKey = "user";

        [Test]
        public void LoadsWithoutWritableStorageContract()
        {
            var serializer = new JsonSaveSerializer();
            var writableStorage = new MemorySaveStorage();
            var saveService = new SaveService(serializer, writableStorage);
            var document = UserSaveDocumentMapper.ToDocument(UserTestData.CreateSnapshot());
            Assert.That(saveService.TrySave(SaveKey, document), Is.True);

            IReadOnlySaveStorage readOnlyStorage = writableStorage;
            var reader = new SaveReader(serializer, readOnlyStorage);

            var loaded = reader.TryLoad(SaveKey, out UserSaveDocument restored);

            Assert.That(loaded, Is.True);
            Assert.That(restored.Identity.UserId, Is.EqualTo("test-user"));
        }
    }
}