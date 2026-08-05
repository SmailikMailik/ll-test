using System;
using LL.Game.Data;
using LL.Game.Data.Configuration;
using LL.Game.Data.Sources;
using LL.Game.Data.Declarations;
using LL.Infrastructure.Compilation;
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
            return Create(
                new ScriptableObjectGameDataSource(manifest),
                new GameDataCompiler());
        }

        internal static IDataLoader<GameDataSnapshot> CreateFromSerialized(
            ISaveSerializer serializer,
            IReadOnlySaveStorage storage)
        {
            var reader = new SaveReader(serializer, storage);
            return Create(new SerializedGameDataSource(reader), new GameDataCompiler());
        }

        private static IDataLoader<GameDataSnapshot> Create(
            IDataSource<GameDataDeclaration> source,
            IDataCompiler<GameDataDeclaration, GameDataSnapshot> compiler)
        {
            return new CompiledDataLoader<GameDataDeclaration, GameDataSnapshot>(source, compiler);
        }
    }
}