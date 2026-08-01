using System;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;

namespace LL.Infrastructure.Saving
{
    internal sealed class SaveService : ISaveService
    {
        private readonly ISaveSerializer _serializer;
        private readonly ISaveStorage _storage;
        private readonly SaveReader _reader;

        internal SaveService(
            ISaveSerializer serializer,
            ISaveStorage storage)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _reader = new SaveReader(_serializer, _storage);
        }

        public bool Exists(string key) => _reader.Exists(key);

        public bool TrySave<T>(string key, T data) where T : class
        {
            return _serializer.TrySerialize(data, out var bytes) &&
                   _storage.TryWrite(key, bytes);
        }

        public bool TryLoad<T>(string key, out T data) where T : class
        {
            return _reader.TryLoad(key, out data);
        }

        public bool TryDelete(string key) => _storage.TryDelete(key);
    }
}