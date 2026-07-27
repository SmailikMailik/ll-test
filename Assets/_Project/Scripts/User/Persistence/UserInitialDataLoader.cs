using System;
using LL.Game.Ranks;
using LL.Loading;
using LL.Saving;
using LL.User.Configuration;
using LL.User.Core;
using VContainer;

namespace LL.User.Persistence
{
    internal sealed class UserInitialDataLoader : IDataLoader<UserInitialData>
    {
        internal const string SaveKey = "user";

        private readonly IUserDefaultsProvider _defaults;
        private readonly ISaveService _saveService;
        private readonly IRankProgression _rankProgression;

        [Inject]
        internal UserInitialDataLoader(
            IUserDefaultsProvider defaults,
            ISaveService saveService,
            IRankProgression rankProgression)
        {
            _defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
        }

        public UserInitialData Load()
        {
            if (_saveService.TryLoad<UserSaveData>(SaveKey, out var savedData) && savedData.IsSupported)
            {
                var initialData = UserSaveDataMapper.ToInitialData(savedData);

                if (savedData.NeedsMigration is false)
                    return initialData;

                var defaults = _defaults.GetDefaults();
                var migratedData = new UserInitialData(
                    initialData.Identity,
                    initialData.Wallet,
                    initialData.Progress,
                    initialData.Cards,
                    defaults.Items,
                    initialData.RewardClaims);
                var migratedRank = migratedData.Progress.ResolveRank(_rankProgression);
                _saveService.TrySave(SaveKey, UserSaveDataMapper.ToSaveData(migratedData, migratedRank));
                return migratedData;
            }

            var defaultData = _defaults.GetDefaults();
            var initialRank = defaultData.Progress.ResolveRank(_rankProgression);
            _saveService.TrySave(SaveKey, UserSaveDataMapper.ToSaveData(defaultData, initialRank));
            return defaultData;
        }
    }
}