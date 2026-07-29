using System;
using LL.Loading;
using LL.Saving;
using LL.User.Configuration;
using LL.User.Core;
using UnityEngine;
using VContainer;

namespace LL.User.Persistence
{
    internal sealed class UserInitialDataLoader : IDataLoader<UserInitialData>
    {
        internal const string SaveKey = "user";

        private readonly IUserDefaultsProvider _defaults;
        private readonly ISaveService _saveService;

        [Inject]
        internal UserInitialDataLoader(
            IUserDefaultsProvider defaults,
            ISaveService saveService)
        {
            _defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        }

        public UserInitialData Load()
        {
            if (_saveService.TryLoad<UserSaveData>(SaveKey, out var savedData))
            {
                if (savedData.Version == UserSaveData.CurrentVersion)
                    return UserSaveDataMapper.ToInitialData(savedData);

                Debug.LogWarning(
                    $"Unsupported user save version {savedData.Version}. " +
                    $"Expected {UserSaveData.CurrentVersion}. Resetting user data.");
            }

            var defaultData = _defaults.GetDefaults();
            _saveService.TrySave(SaveKey, UserSaveDataMapper.ToSaveData(defaultData));
            return defaultData;
        }
    }
}