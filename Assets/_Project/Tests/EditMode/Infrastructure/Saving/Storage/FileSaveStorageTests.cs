using System;
using System.IO;
using LL.Infrastructure.Saving.Storage;
using NUnit.Framework;

namespace LL.Tests.EditMode.Infrastructure.Saving.Storage
{
    internal sealed class FileSaveStorageTests
    {
        private string _directoryPath;

        [SetUp]
        public void SetUp()
        {
            _directoryPath = Path.Combine(
                Path.GetTempPath(),
                $"ll-persistence-tests-{Guid.NewGuid():N}");
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directoryPath))
                Directory.Delete(_directoryPath, true);
        }

        [Test]
        public void AtomicallyReplacesExistingData()
        {
            var storage = new FileSaveStorage(_directoryPath);

            Assert.That(storage.TryWrite("user", new byte[] { 1 }), Is.True);
            Assert.That(storage.TryWrite("user", new byte[] { 2, 3 }), Is.True);

            Assert.That(storage.TryRead("user", out var bytes), Is.True);
            Assert.That(bytes, Is.EqualTo(new byte[] { 2, 3 }));
            Assert.That(File.Exists(Path.Combine(_directoryPath, "user.save.tmp")), Is.False);
        }
    }
}