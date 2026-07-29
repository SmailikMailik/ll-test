using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace LL.Infrastructure.Saving.Serialization
{
    internal sealed class JsonSaveSerializer : ISaveSerializer
    {
        private static readonly Encoding _encoding = new UTF8Encoding(false);

        private static readonly JsonSerializerSettings _settings = new()
        {
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Error,
            TypeNameHandling = TypeNameHandling.None
        };

        public bool TrySerialize<T>(T data, out byte[] bytes) where T : class
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            try
            {
                var json = JsonConvert.SerializeObject(data, _settings);
                bytes = _encoding.GetBytes(json);
                return true;
            }
            catch (Exception exception) when (IsSerializationException(exception))
            {
                bytes = null;
                Debug.LogError($"Failed to serialize {typeof(T).Name}: {exception.Message}");
                return false;
            }
        }

        public bool TryDeserialize<T>(byte[] bytes, out T data) where T : class
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            try
            {
                var json = _encoding.GetString(bytes);
                data = JsonConvert.DeserializeObject<T>(json, _settings);
                return data != null;
            }
            catch (Exception exception) when (IsSerializationException(exception))
            {
                data = null;
                Debug.LogError($"Failed to deserialize {typeof(T).Name}: {exception.Message}");
                return false;
            }
        }

        private static bool IsSerializationException(Exception exception)
        {
            return exception is JsonException
                or ArgumentException
                or NotSupportedException;
        }
    }
}