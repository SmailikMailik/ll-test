using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Game.Promotions;
using LL.Rewards;
using LL.Saving;
using LL.User.Core.Cards;
using LL.User.Core.Identity;
using LL.User.Core.Promotions;
using LL.User.Core.Progress;
using LL.User.Core.Items;
using LL.User.Core.Rewards;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.User.Persistence
{
    internal sealed class UserSaveController : IInitializable, IDisposable
    {
        private readonly UserIdentity _identity;
        private readonly IUserItems _items;
        private readonly IUserProgress _progress;
        private readonly ISaveService _saveService;
        private readonly IUserCards _cards;
        private readonly IUserPromotionOrder _promotionOrder;
        private readonly IUserRewardClaims _rewardClaims;
        private readonly List<IDisposable> _subscriptions = new();

        private readonly Dictionary<ItemId, int> _itemAmounts;
        private readonly Dictionary<CardId, int> _cardAmounts;
        private readonly HashSet<RewardBundleId> _claimedRewardIds;
        private int _rank;
        private int _experience;
        private PromotionRequirementId _promotionOrderRequirementId;
        private long _promotionOrderDeadlineUnixMilliseconds;
        private bool _isPromotionOrderCompleted;

        [Inject]
        internal UserSaveController(
            UserIdentity identity,
            ItemsInitialData items,
            CardsInitialData cards,
            PromotionOrderInitialData promotionOrder,
            IUserItems userItems,
            IUserCards userCards,
            IUserPromotionOrder userPromotionOrder,
            IUserRewardClaims rewardClaims,
            IUserProgress progress,
            ISaveService saveService)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (promotionOrder == null)
                throw new ArgumentNullException(nameof(promotionOrder));

            _items = userItems ?? throw new ArgumentNullException(nameof(userItems));
            _cards = userCards ?? throw new ArgumentNullException(nameof(userCards));
            _promotionOrder = userPromotionOrder ?? throw new ArgumentNullException(nameof(userPromotionOrder));
            _rewardClaims = rewardClaims ?? throw new ArgumentNullException(nameof(rewardClaims));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));

            _itemAmounts = items.Amounts.ToDictionary(
                item => item.Id,
                item => item.Amount);
            _cardAmounts = cards.Stacks.ToDictionary(
                stack => stack.Id,
                stack => stack.Amount);
            _claimedRewardIds = new HashSet<RewardBundleId>(rewardClaims.ClaimedIds);
            _rank = progress.Rank;
            _experience = progress.Experience;
            _promotionOrderRequirementId = promotionOrder.RequirementId;
            _promotionOrderDeadlineUnixMilliseconds = promotionOrder.DeadlineUnixMilliseconds;
            _isPromotionOrderCompleted = promotionOrder.IsCompleted;
        }

        public void Initialize()
        {
            foreach (var id in _itemAmounts.Keys.ToArray())
            {
                var itemId = id;
                _subscriptions.Add(_items
                    .ObserveAmount(itemId)
                    .Subscribe(value => UpdateItemAndSave(itemId, value)));
            }

            foreach (var id in _cardAmounts.Keys.ToArray())
            {
                var cardId = id;
                _subscriptions.Add(_cards
                    .ObserveAmount(cardId)
                    .Subscribe(value => UpdateCardAndSave(cardId, value)));
            }

            _subscriptions.Add(_progress.RankChanged
                .Subscribe(value => UpdateAndSave(ref _rank, value)));
            _subscriptions.Add(_progress.ExperienceChanged
                .Subscribe(value => UpdateAndSave(ref _experience, value)));
            _subscriptions.Add(_promotionOrder.Changed.Subscribe(_ => UpdatePromotionOrderAndSave()));
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

        private void UpdateItemAndSave(ItemId id, int value)
        {
            if (_itemAmounts[id] == value)
                return;

            _itemAmounts[id] = value;
            Save();
        }

        private void UpdateCardAndSave(CardId id, int value)
        {
            if (_cardAmounts[id] == value)
                return;

            _cardAmounts[id] = value;
            Save();
        }

        private void AddClaimAndSave(RewardBundleId id)
        {
            if (_claimedRewardIds.Add(id))
                Save();
        }

        private void UpdatePromotionOrderAndSave()
        {
            var requirementId = _promotionOrder.RequirementId;
            var deadlineUnixMilliseconds = _promotionOrder.DeadlineUnixMilliseconds;
            var isCompleted = _promotionOrder.IsCompleted;

            if (_promotionOrderRequirementId.Equals(requirementId) &&
                _promotionOrderDeadlineUnixMilliseconds == deadlineUnixMilliseconds &&
                _isPromotionOrderCompleted == isCompleted)
            {
                return;
            }

            _promotionOrderRequirementId = requirementId;
            _promotionOrderDeadlineUnixMilliseconds = deadlineUnixMilliseconds;
            _isPromotionOrderCompleted = isCompleted;
            Save();
        }

        private void Save()
        {
            var data = UserSaveDataMapper.ToSaveData(
                _identity,
                new ProgressSaveData(_rank, _experience),
                _cardAmounts.Select(pair => new CardStack(pair.Key, pair.Value)),
                _itemAmounts.Select(pair => new ItemAmount(pair.Key, pair.Value)),
                new PromotionOrderInitialData(
                    _promotionOrderRequirementId,
                    _promotionOrderDeadlineUnixMilliseconds,
                    _isPromotionOrderCompleted),
                _claimedRewardIds);

            _saveService.TrySave(UserInitialDataLoader.SaveKey, data);
        }
    }
}