using System;
using UnityEngine;

namespace LL.Infrastructure.Saving.Storage
{
    internal sealed class PlayerPrefsSaveStorage : ISaveStorage
    {
        private readonly string _keyPrefix;

        internal PlayerPrefsSaveStorage(string keyPrefix = "")
        {
            _keyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));
        }

        public bool Exists(string key) => PlayerPrefs.HasKey(GetStorageKey(key));

        public bool TryWrite(string key, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            try
            {
                PlayerPrefs.SetString(GetStorageKey(key), Convert.ToBase64String(data));
                PlayerPrefs.Save();
                return true;
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                Debug.LogError($"Failed to write PlayerPrefs value '{key}': {exception.Message}");
                return false;
            }
        }

        public bool TryRead(string key, out byte[] data)
        {
            var storageKey = GetStorageKey(key);
            data = null;

            if (PlayerPrefs.HasKey(storageKey) is false)
                return false;

            try
            {
                data = Convert.FromBase64String(PlayerPrefs.GetString(storageKey));
                return true;
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                Debug.LogError($"Failed to read PlayerPrefs value '{key}': {exception.Message}");
                return false;
            }
        }

        public bool TryDelete(string key)
        {
            try
            {
                PlayerPrefs.DeleteKey(GetStorageKey(key));
                PlayerPrefs.Save();
                return true;
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                Debug.LogError($"Failed to delete PlayerPrefs value '{key}': {exception.Message}");
                return false;
            }
        }

        private string GetStorageKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Storage key cannot be empty.", nameof(key));

            return _keyPrefix + key;
        }

        private static bool IsStorageException(Exception exception)
        {
            return exception is ArgumentException
                or FormatException
                or OverflowException;
        }
    }
}