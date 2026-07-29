using System;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;

namespace LL.Infrastructure.Saving
{
    internal sealed class SaveService : ISaveService
    {
        private readonly ISaveSerializer _serializer;
        private readonly ISaveStorage _storage;

        internal SaveService(
            ISaveSerializer serializer,
            ISaveStorage storage)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public bool Exists(string key) => _storage.Exists(key);

        public bool TrySave<T>(string key, T data) where T : class
        {
            return _serializer.TrySerialize(data, out var bytes) &&
                   _storage.TryWrite(key, bytes);
        }

        public bool TryLoad<T>(string key, out T data) where T : class
        {
            data = null;

            return _storage.TryRead(key, out var bytes) &&
                   _serializer.TryDeserialize(bytes, out data);
        }

        public bool TryDelete(string key) => _storage.TryDelete(key);
    }
}