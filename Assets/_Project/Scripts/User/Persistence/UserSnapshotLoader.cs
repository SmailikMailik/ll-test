using System;
using LL.Infrastructure.Loading;
using LL.Infrastructure.Saving;
using LL.User.Configuration;
using LL.User.Persistence.SaveData;
using LL.User.Snapshots;
using UnityEngine;
using VContainer;

namespace LL.User.Persistence
{
    internal sealed class UserSnapshotLoader : IDataLoader<UserSnapshot>
    {
        internal const string SaveKey = "user";

        private readonly IUserDefaultsProvider _defaultsProvider;
        private readonly ISaveService _saveService;

        [Inject]
        internal UserSnapshotLoader(
            IUserDefaultsProvider defaultsProvider,
            ISaveService saveService)
        {
            _defaultsProvider = defaultsProvider ?? throw new ArgumentNullException(nameof(defaultsProvider));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        }

        public UserSnapshot Load()
        {
            if (_saveService.TryLoad<UserSaveData>(SaveKey, out var savedData))
            {
                if (savedData.Version == UserSaveData.CurrentVersion)
                    return UserSaveDataMapper.ToSnapshot(savedData);

                Debug.LogWarning(
                    $"Unsupported user save version {savedData.Version}. " +
                    $"Expected {UserSaveData.CurrentVersion}. Resetting user data.");
            }

            var defaultSnapshot = _defaultsProvider.GetDefaultSnapshot();
            _saveService.TrySave(SaveKey, UserSaveDataMapper.ToSaveData(defaultSnapshot));
            return defaultSnapshot;
        }
    }
}