using System.Collections.Generic;
using LL.Infrastructure.Saving.Storage;

namespace LL.Tests.EditMode.Persistence
{
    internal sealed class MemorySaveStorage : ISaveStorage
    {
        private readonly Dictionary<string, byte[]> _entries = new();

        public bool Exists(string key) => _entries.ContainsKey(key);

        public bool TryWrite(string key, byte[] data)
        {
            _entries[key] = (byte[])data.Clone();
            return true;
        }

        public bool TryRead(string key, out byte[] data)
        {
            if (_entries.TryGetValue(key, out var stored) is false)
            {
                data = null;
                return false;
            }

            data = (byte[])stored.Clone();
            return true;
        }

        public bool TryDelete(string key) => _entries.Remove(key);
    }
}