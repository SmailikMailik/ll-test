using System.Linq;
using LL.Game.Items;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Tests.EditMode.Infrastructure.Saving.Storage;
using LL.Tests.EditMode.TestData;
using LL.User.Persistence;
using LL.User.Persistence.Documents;
using NUnit.Framework;

namespace LL.Tests.EditMode.User.Persistence
{
    internal sealed class SerializedUserSaveRepositoryTests
    {
        [Test]
        public void RoundTripPreservesUser()
        {
            var source = UserTestData.CreateSnapshot(items: new[] { new ItemAmount(ItemIds.Soft, 15) });
            var saveService = new SaveService(new JsonSaveSerializer(), new MemorySaveStorage());
            var repository = new SerializedUserSaveRepository(saveService);

            Assert.That(repository.Save(source), Is.True);

            var loadResult = repository.Load();
            var loadedDocument = UserSaveDocumentMapper.ToDocument(loadResult.Snapshot);
            var loadedSoftAmount = loadResult.Snapshot.Items.Amounts
                .Single(item => item.Id.Equals(ItemIds.Soft)).Amount;

            Assert.That(loadResult.Status, Is.EqualTo(UserLoadStatus.Loaded));
            Assert.That(loadedDocument.Version, Is.EqualTo(UserSaveDocument.CurrentVersion));
            Assert.That(loadResult.Snapshot.Identity.UserId, Is.EqualTo(source.Identity.UserId));
            Assert.That(loadedSoftAmount, Is.EqualTo(15));
        }
    }
}