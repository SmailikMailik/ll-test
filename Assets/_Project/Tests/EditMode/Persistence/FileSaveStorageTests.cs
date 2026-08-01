using System;
using System.IO;
using LL.Infrastructure.Saving.Storage;
using NUnit.Framework;

namespace LL.Tests.EditMode.Persistence
{
    internal sealed class FileSaveStorageTests
    {
        [Test]
        public void AtomicallyReplacesExistingData()
        {
            var directoryPath = Path.Combine(Path.GetTempPath(), $"ll-persistence-tests-{Guid.NewGuid():N}");

            try
            {
                var storage = new FileSaveStorage(directoryPath);

                Assert.That(storage.TryWrite("user", new byte[] { 1 }), Is.True);
                Assert.That(storage.TryWrite("user", new byte[] { 2, 3 }), Is.True);
                Assert.That(storage.TryRead("user", out var bytes), Is.True);
                Assert.That(bytes, Is.EqualTo(new byte[] { 2, 3 }));
                Assert.That(File.Exists(Path.Combine(directoryPath, "user.save.tmp")), Is.False);
            }
            finally
            {
                if (Directory.Exists(directoryPath))
                    Directory.Delete(directoryPath, true);
            }
        }
    }
}