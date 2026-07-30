using System;
using LL.Game.Data;
using LL.Game.Data.Configuration;
using LL.Game.Data.Sources;
using LL.Infrastructure.Loading;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;

namespace LL.Composition.Factories
{
    internal static class GameDataLoaderFactory
    {
        internal static IDataLoader<GameDataSnapshot> CreateFromScriptableObjects(GameDataManifestConfig manifest)
        {
            return new GameDataLoader(
                new ScriptableObjectGameDataSource(manifest),
                new GameDataCompiler());
        }

        internal static IDataLoader<GameDataSnapshot> CreateFromJsonFile(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Game data directory path cannot be empty.", nameof(directoryPath));

            return CreateSerializedLoader(new FileSaveStorage(directoryPath, ".json"));
        }

        internal static IDataLoader<GameDataSnapshot> CreateFromJsonPlayerPrefs(string keyPrefix = "")
        {
            return CreateSerializedLoader(new PlayerPrefsSaveStorage(keyPrefix));
        }

        private static IDataLoader<GameDataSnapshot> CreateSerializedLoader(ISaveStorage storage)
        {
            var serializer = new JsonSaveSerializer();
            var saveService = new SaveService(serializer, storage);

            return new GameDataLoader(
                new SerializedGameDataSource(saveService),
                new GameDataCompiler());
        }
    }
}