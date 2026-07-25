using System;
using System.Collections.Generic;
using LL.Saving;
using LL.User.Core.ExperienceCards;
using LL.User.Core.Identity;
using LL.User.Core.Progress;
using LL.User.Core.Wallet;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.User.Persistence
{
    internal sealed class UserSaveController : IInitializable, IDisposable
    {
        private readonly UserIdentity _identity;
        private readonly IUserWallet _wallet;
        private readonly IUserProgress _progress;
        private readonly ISaveService _saveService;
        private readonly ExperienceCardsInitialData _experienceCards;
        private readonly List<IDisposable> _subscriptions = new();

        private int _softAmount;
        private int _hardAmount;
        private int _masterPointAmount;
        private int _totalExperience;

        [Inject]
        internal UserSaveController(
            UserIdentity identity,
            WalletInitialData walletInitialData,
            ProgressInitialData progressInitialData,
            ExperienceCardsInitialData experienceCards,
            IUserWallet wallet,
            IUserProgress progress,
            ISaveService saveService)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));

            if (walletInitialData == null)
                throw new ArgumentNullException(nameof(walletInitialData));

            if (progressInitialData == null)
                throw new ArgumentNullException(nameof(progressInitialData));

            _experienceCards = experienceCards
                ?? throw new ArgumentNullException(nameof(experienceCards));

            _wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));

            _softAmount = walletInitialData.SoftAmount;
            _hardAmount = walletInitialData.HardAmount;
            _masterPointAmount = walletInitialData.MasterPointAmount;
            _totalExperience = progressInitialData.TotalExperience;
        }

        public void Initialize()
        {
            _subscriptions.Add(_wallet
                .ObserveAmount(CurrencyType.Soft)
                .Subscribe(value => UpdateAndSave(ref _softAmount, value)));

            _subscriptions.Add(_wallet
                .ObserveAmount(CurrencyType.Hard)
                .Subscribe(value => UpdateAndSave(ref _hardAmount, value)));

            _subscriptions.Add(_wallet
                .ObserveAmount(CurrencyType.MasterPoint)
                .Subscribe(value => UpdateAndSave(ref _masterPointAmount, value)));

            _subscriptions.Add(_progress.TotalExperience
                .Subscribe(value => UpdateAndSave(ref _totalExperience, value)));
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
        }

        private void UpdateAndSave(ref int field, int value)
        {
            if (field == value)
                return;

            field = value;
            Save();
        }

        private void Save()
        {
            var data = UserSaveDataMapper.ToSaveData(
                _identity,
                _softAmount,
                _hardAmount,
                _masterPointAmount,
                _totalExperience,
                _experienceCards);

            _saveService.TrySave(UserInitialDataLoader.SaveKey, data);
        }
    }
}