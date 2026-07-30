using System;
using LL.Infrastructure.Loading;
using LL.Infrastructure.Saving;

namespace LL.Game.Data.Persistence
{
    internal sealed class SerializedGameDataLoader : IDataLoader<GameDataSnapshot>
    {
        internal const string DataKey = "game-data";

        private readonly ISaveService _saveService;

        internal SerializedGameDataLoader(ISaveService saveService)
        {
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        }

        public GameDataSnapshot Load()
        {
            if (_saveService.TryLoad<GameDataDocument>(DataKey, out var document) is false)
                throw new InvalidOperationException($"Game data '{DataKey}' could not be loaded.");

            if (document.Version != GameDataDocument.CurrentVersion)
            {
                throw new NotSupportedException(
                    $"Unsupported game data version {document.Version}. " +
                    $"Expected {GameDataDocument.CurrentVersion}.");
            }

            return GameDataDocumentMapper.ToSnapshot(document);
        }
    }
}