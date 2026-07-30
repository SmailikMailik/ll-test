using System;
using LL.Game.Data.Declarations;
using LL.Game.Data.Persistence;
using LL.Game.Data.Persistence.Documents;
using LL.Infrastructure.Saving;

namespace LL.Game.Data.Sources
{
    internal sealed class SerializedGameDataSource : IGameDataSource
    {
        internal const string DataKey = "game-data";

        private readonly ISaveService _saveService;

        internal SerializedGameDataSource(ISaveService saveService)
        {
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        }

        public GameDataDeclaration Read()
        {
            if (_saveService.TryLoad<GameDataDocument>(DataKey, out var document) is false)
                throw new InvalidOperationException($"Game data '{DataKey}' could not be loaded.");

            if (document.Version != GameDataDocument.CurrentVersion)
            {
                throw new NotSupportedException(
                    $"Unsupported game data version {document.Version}. " +
                    $"Expected {GameDataDocument.CurrentVersion}.");
            }

            return GameDataDocumentMapper.ToDeclaration(document);
        }
    }
}