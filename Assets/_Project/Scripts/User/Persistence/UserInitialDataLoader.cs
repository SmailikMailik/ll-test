using System;
using LL.Saving;
using LL.User.Core;
using VContainer;

namespace LL.User.Persistence
{
    internal sealed class UserInitialDataLoader : IDataLoader<UserInitialData>
    {
        internal const string SaveKey = "user";

        private readonly IDefaultDataLoader<UserInitialData> _defaults;
        private readonly ISaveService _saveService;

        [Inject]
        internal UserInitialDataLoader(
            IDefaultDataLoader<UserInitialData> defaults,
            ISaveService saveService)
        {
            _defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        }

        public UserInitialData Load()
        {
            if (_saveService.TryLoad<UserSaveData>(SaveKey, out var savedData) && savedData.IsSupported)
                return UserSaveDataMapper.ToInitialData(savedData);

            var defaultData = _defaults.Load();
            _saveService.TrySave(SaveKey, UserSaveDataMapper.ToSaveData(defaultData));
            return defaultData;
        }
    }
}