using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;

namespace LL.Saving
{
    internal sealed class JsonFileSaveService : ISaveService
    {
        private const string FileExtension = ".json";

        private static readonly Encoding _encoding = new UTF8Encoding(false);

        private static readonly JsonSerializerSettings _serializerSettings = new()
        {
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Error,
            TypeNameHandling = TypeNameHandling.None
        };

        private readonly string _directoryPath;

        [Inject]
        internal JsonFileSaveService() : this(Path.Combine(Application.persistentDataPath, "Saves")) { }

        internal JsonFileSaveService(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Save directory path cannot be empty", nameof(directoryPath));

            _directoryPath = directoryPath;
        }

        public bool Exists(string key)
        {
            return File.Exists(GetFilePath(key));
        }

        public bool TrySave<T>(string key, T data) where T : class
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var filePath = GetFilePath(key);
            var temporaryPath = filePath + ".tmp";

            try
            {
                Directory.CreateDirectory(_directoryPath);

                var json = JsonConvert.SerializeObject(data, _serializerSettings);
                File.WriteAllText(temporaryPath, json, _encoding);
                File.Copy(temporaryPath, filePath, true);
                File.Delete(temporaryPath);

                return true;
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                TryDeleteTemporaryFile(temporaryPath);
                Debug.LogError($"Failed to save '{key}': {exception.Message}");
                return false;
            }
        }

        public bool TryLoad<T>(string key, out T data) where T : class
        {
            var filePath = GetFilePath(key);
            data = null;

            if (File.Exists(filePath) is false)
                return false;

            try
            {
                var json = File.ReadAllText(filePath, _encoding);
                data = JsonConvert.DeserializeObject<T>(json, _serializerSettings);
                return data != null;
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                Debug.LogError($"Failed to load '{key}': {exception.Message}");
                return false;
            }
        }

        public bool TryDelete(string key)
        {
            var filePath = GetFilePath(key);

            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);

                return true;
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                Debug.LogError($"Failed to delete '{key}': {exception.Message}");
                return false;
            }
        }

        private string GetFilePath(string key)
        {
            ValidateKey(key);
            return Path.Combine(_directoryPath, key + FileExtension);
        }

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Save key cannot be empty", nameof(key));

            foreach (var character in key)
            {
                if (char.IsLetterOrDigit(character) || character is '_' or '-')
                    continue;

                throw new ArgumentException(
                    "Save key can contain only letters, digits, underscores and hyphens",
                    nameof(key));
            }
        }

        private static void TryDeleteTemporaryFile(string temporaryPath)
        {
            try
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                Debug.LogWarning($"Failed to delete temporary save file: {exception.Message}");
            }
        }

        private static bool IsStorageException(Exception exception)
        {
            return exception is IOException
                or UnauthorizedAccessException
                or ArgumentException
                or JsonException
                or NotSupportedException;
        }
    }
}