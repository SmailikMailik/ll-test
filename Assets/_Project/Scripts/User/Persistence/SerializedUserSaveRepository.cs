using System;
using LL.Infrastructure.Saving;
using LL.User.Persistence.Documents;
using LL.User.Snapshots;

namespace LL.User.Persistence
{
    internal sealed class SerializedUserSaveRepository : IUserSaveRepository
    {
        internal const string SaveKey = "user";

        private readonly ISaveService _saveService;

        internal SerializedUserSaveRepository(ISaveService saveService)
        {
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        }

        public UserLoadResult Load()
        {
            if (_saveService.Exists(SaveKey) is false)
                return UserLoadResult.Failed(UserLoadStatus.NotFound);

            if (_saveService.TryLoad<UserSaveDocument>(SaveKey, out var document) is false || document is null)
                return UserLoadResult.Failed(UserLoadStatus.Corrupted);

            if (document.Version != UserSaveDocument.CurrentVersion)
                return UserLoadResult.Failed(UserLoadStatus.UnsupportedVersion, document.Version);

            try
            {
                return UserLoadResult.Loaded(UserSaveDocumentMapper.ToSnapshot(document));
            }
            catch (Exception)
            {
                return UserLoadResult.Failed(UserLoadStatus.Corrupted);
            }
        }

        public bool Save(UserSnapshot snapshot)
        {
            return _saveService.TrySave(SaveKey, UserSaveDocumentMapper.ToDocument(snapshot));
        }

        public bool Exists() => _saveService.Exists(SaveKey);

        public bool Delete() => _saveService.TryDelete(SaveKey);
    }
}