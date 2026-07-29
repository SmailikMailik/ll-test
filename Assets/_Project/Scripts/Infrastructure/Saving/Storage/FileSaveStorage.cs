using System;
using System.IO;
using UnityEngine;

namespace LL.Infrastructure.Saving.Storage
{
    internal sealed class FileSaveStorage : ISaveStorage
    {
        private const string FileExtension = ".save";

        private readonly string _directoryPath;

        internal FileSaveStorage() : this(Path.Combine(Application.persistentDataPath, "Saves")) { }

        internal FileSaveStorage(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Save directory path cannot be empty", nameof(directoryPath));

            _directoryPath = directoryPath;
        }

        public bool Exists(string key)
        {
            return File.Exists(GetFilePath(key));
        }

        public bool TryWrite(string key, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var filePath = GetFilePath(key);
            var temporaryPath = filePath + ".tmp";

            try
            {
                Directory.CreateDirectory(_directoryPath);

                File.WriteAllBytes(temporaryPath, data);
                File.Copy(temporaryPath, filePath, true);
                File.Delete(temporaryPath);

                return true;
            }
            catch (Exception exception) when (IsFileSystemException(exception))
            {
                TryDeleteTemporaryFile(temporaryPath);
                Debug.LogError($"Failed to write save '{key}': {exception.Message}");
                return false;
            }
        }

        public bool TryRead(string key, out byte[] data)
        {
            var filePath = GetFilePath(key);
            data = null;

            if (File.Exists(filePath) is false)
                return false;

            try
            {
                data = File.ReadAllBytes(filePath);
                return true;
            }
            catch (Exception exception) when (IsFileSystemException(exception))
            {
                Debug.LogError($"Failed to read save '{key}': {exception.Message}");
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
            catch (Exception exception) when (IsFileSystemException(exception))
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
            catch (Exception exception) when (IsFileSystemException(exception))
            {
                Debug.LogWarning($"Failed to delete temporary save file: {exception.Message}");
            }
        }

        private static bool IsFileSystemException(Exception exception)
        {
            return exception is IOException
                or UnauthorizedAccessException
                or ArgumentException
                or NotSupportedException;
        }
    }
}