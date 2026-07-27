using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Currencies;
using LL.Game.Items;
using LL.Rewards;
using LL.Saving;
using LL.User.Core.Cards;
using LL.User.Core.Identity;
using LL.User.Core.Items;
using LL.User.Core.Progress;
using LL.User.Core.Rewards;
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
        private readonly IUserCards _cards;
        private readonly IUserItems _items;
        private readonly IUserRewardClaims _rewardClaims;
        private readonly List<IDisposable> _subscriptions = new();

        private readonly Dictionary<CurrencyId, int> _currencyAmounts;
        private readonly Dictionary<CardId, int> _cardAmounts;
        private readonly Dictionary<ItemId, int> _itemAmounts;
        private readonly HashSet<RewardBundleId> _claimedRewardIds;
        private int _rank;
        private int _experience;

        [Inject]
        internal UserSaveController(
            UserIdentity identity,
            WalletInitialData walletInitialData,
            CardsInitialData cards,
            ItemsInitialData items,
            IUserWallet wallet,
            IUserCards userCards,
            IUserItems userItems,
            IUserRewardClaims rewardClaims,
            IUserProgress progress,
            ISaveService saveService)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));

            if (walletInitialData == null)
                throw new ArgumentNullException(nameof(walletInitialData));

            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            _cards = userCards ?? throw new ArgumentNullException(nameof(userCards));
            _items = userItems ?? throw new ArgumentNullException(nameof(userItems));
            _rewardClaims = rewardClaims ?? throw new ArgumentNullException(nameof(rewardClaims));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));

            _currencyAmounts = walletInitialData.Balances.ToDictionary(
                balance => balance.Id,
                balance => balance.Amount);
            _cardAmounts = cards.Stacks.ToDictionary(
                stack => stack.Id,
                stack => stack.Amount);
            _itemAmounts = items.Stacks.ToDictionary(
                stack => stack.Id,
                stack => stack.Amount);
            _claimedRewardIds = new HashSet<RewardBundleId>(rewardClaims.ClaimedIds);
            _rank = progress.Rank;
            _experience = progress.Experience;
        }

        public void Initialize()
        {
            foreach (var id in _currencyAmounts.Keys.ToArray())
            {
                var currencyId = id;
                _subscriptions.Add(_wallet
                    .ObserveAmount(currencyId)
                    .Subscribe(value => UpdateCurrencyAndSave(currencyId, value)));
            }

            foreach (var id in _cardAmounts.Keys.ToArray())
            {
                var cardId = id;
                _subscriptions.Add(_cards
                    .ObserveAmount(cardId)
                    .Subscribe(value => UpdateCardAndSave(cardId, value)));
            }

            foreach (var id in _itemAmounts.Keys.ToArray())
            {
                var itemId = id;
                _subscriptions.Add(_items
                    .ObserveAmount(itemId)
                    .Subscribe(value => UpdateItemAndSave(itemId, value)));
            }

            _subscriptions.Add(_progress.RankChanged
                .Subscribe(value => UpdateAndSave(ref _rank, value)));
            _subscriptions.Add(_progress.ExperienceChanged
                .Subscribe(value => UpdateAndSave(ref _experience, value)));
            _subscriptions.Add(_rewardClaims.RewardClaimed.Subscribe(AddClaimAndSave));
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

        private void UpdateCurrencyAndSave(CurrencyId id, int value)
        {
            if (_currencyAmounts[id] == value)
                return;

            _currencyAmounts[id] = value;
            Save();
        }

        private void UpdateCardAndSave(CardId id, int value)
        {
            if (_cardAmounts[id] == value)
                return;

            _cardAmounts[id] = value;
            Save();
        }

        private void UpdateItemAndSave(ItemId id, int value)
        {
            if (_itemAmounts[id] == value)
                return;

            _itemAmounts[id] = value;
            Save();
        }

        private void AddClaimAndSave(RewardBundleId id)
        {
            if (_claimedRewardIds.Add(id))
                Save();
        }

        private void Save()
        {
            var data = UserSaveDataMapper.ToSaveData(
                _identity,
                new ProgressSaveData(_rank, _experience),
                _cardAmounts.Select(pair => new CardStack(pair.Key, pair.Value)),
                _currencyAmounts.Select(pair => new CurrencyBalance(pair.Key, pair.Value)),
                _itemAmounts.Select(pair => new ItemStack(pair.Key, pair.Value)),
                _claimedRewardIds);

            _saveService.TrySave(UserInitialDataLoader.SaveKey, data);
        }
    }
}