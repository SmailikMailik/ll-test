using LL.Game.Items;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Tests.EditMode;
using LL.User.Persistence;
using NUnit.Framework;

namespace LL.Tests.EditMode.Persistence
{
    internal sealed class SerializedUserSaveRepositoryTests
    {
        [Test]
        public void RoundTripPreservesUser()
        {
            var storage = new MemorySaveStorage();
            var repository = new SerializedUserSaveRepository(new SaveService(new JsonSaveSerializer(), storage));
            var source = TestDataFactory.CreateUserSnapshot(items: new[] { new ItemAmount(ItemIds.Soft, 15) });

            Assert.That(repository.Save(source), Is.True);

            var result = repository.Load();

            Assert.That(result.Status, Is.EqualTo(UserLoadStatus.Loaded));
            Assert.That(UserSaveDocumentMapper.ToDocument(result.Snapshot).Version, Is.EqualTo(1));
            Assert.That(result.Snapshot.Identity.UserId, Is.EqualTo(source.Identity.UserId));
            Assert.That(result.Snapshot.Items.Amounts[0].Amount, Is.EqualTo(15));
        }
    }
}