using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.RankUp;
using LL.Game.Ranks;
using LL.User.Defaults;
using LL.User.Snapshots;
using LL.Validation;
using VContainer;

namespace LL.User.Persistence
{
    internal sealed class UserSnapshotReconciler
    {
        private readonly UserDefaultsSnapshot _defaults;
        private readonly HeroCatalog _heroes;
        private readonly IRankProgression _rankProgression;
        private readonly RankUpCatalog _rankUps;
        private readonly TimeProvider _timeProvider;

        [Inject]
        internal UserSnapshotReconciler(
            UserDefaultsSnapshot defaults,
            HeroCatalog heroes,
            IRankProgression rankProgression,
            RankUpCatalog rankUps,
            TimeProvider timeProvider)
        {
            _defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
            _heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _rankUps = rankUps ?? throw new ArgumentNullException(nameof(rankUps));
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        internal UserReconciliationResult Reconcile(UserSnapshot snapshot)
        {
            if (snapshot is null ||
                _rankProgression.TryGetProgress(snapshot.Progress.RankId, snapshot.Progress.Experience, out _) is false)
            {
                return UserReconciliationResult.Incompatible();
            }

            var heroSelection = ReconcileHeroSelection(snapshot.HeroSelection, out var heroSelectionChanged);
            var rankUpQuest = ReconcileRankUpQuest(
                snapshot.Progress,
                snapshot.RankUpQuest,
                out var rankUpQuestChanged);
            var items = ReconcileItems(snapshot.Items, out var itemsChanged);

            if (heroSelectionChanged is false && rankUpQuestChanged is false && itemsChanged is false)
                return UserReconciliationResult.Unchanged(snapshot);

            return UserReconciliationResult.Changed(
                new UserSnapshot(snapshot.Identity, heroSelection, snapshot.Progress, rankUpQuest, items));
        }

        private UserHeroSelectionSnapshot ReconcileHeroSelection(
            UserHeroSelectionSnapshot heroSelection,
            out bool changed)
        {
            changed = _heroes.TryGetHero(heroSelection.HeroId, out _) is false;

            return changed
                ? _defaults.CreateUserSnapshot().HeroSelection
                : heroSelection;
        }

        private UserItemsSnapshot ReconcileItems(
            UserItemsSnapshot items,
            out bool changed)
        {
            var amounts = items.Amounts.ToList();
            var itemIds = new HashSet<ItemId>(amounts.Select(amount => amount.Id));
            changed = false;

            foreach (var defaultAmount in _defaults.CreateUserSnapshot().Items.Amounts)
            {
                if (itemIds.Add(defaultAmount.Id) is false)
                    continue;

                amounts.Add(defaultAmount);
                changed = true;
            }

            return changed
                ? new UserItemsSnapshot(amounts)
                : items;
        }

        private UserRankUpQuestSnapshot ReconcileRankUpQuest(
            UserProgressSnapshot progress,
            UserRankUpQuestSnapshot rankUpQuest,
            out bool changed)
        {
            changed = false;

            if (ValidationChecks.IsEmpty(rankUpQuest.QuestId.Value))
                return rankUpQuest;

            var isExpired =
                rankUpQuest.IsCompleted is false &&
                rankUpQuest.DeadlineUnixMilliseconds <= _timeProvider.GetUtcNow().ToUnixTimeMilliseconds();
            var matchesCurrentRank =
                _rankUps.TryGetDefinition(progress.RankId, out var definition) &&
                definition.Quest.QuestId.Equals(rankUpQuest.QuestId);
            var canUseQuest = _rankProgression.CanRankUp(progress.RankId, progress.Experience);

            if (isExpired is false && matchesCurrentRank && canUseQuest)
                return rankUpQuest;

            changed = true;
            return UserRankUpQuestSnapshot.Empty;
        }
    }
}