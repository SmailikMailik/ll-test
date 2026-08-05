using System;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;

namespace LL.Infrastructure.Saving
{
    internal sealed class SaveReader : IReadOnlySaveService
    {
        private readonly ISaveSerializer _serializer;
        private readonly IReadOnlySaveStorage _storage;

        internal SaveReader(
            ISaveSerializer serializer,
            IReadOnlySaveStorage storage)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public bool Exists(string key) => _storage.Exists(key);

        public bool TryLoad<T>(string key, out T data) where T : class
        {
            data = null;

            return _storage.TryRead(key, out var bytes) &&
                   _serializer.TryDeserialize(bytes, out data);
        }
    }
}