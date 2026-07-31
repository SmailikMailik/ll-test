using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IRankProgression _rankProgression;
        private readonly RankUpCatalog _rankUps;

        [Inject]
        internal UserSnapshotReconciler(
            UserDefaultsSnapshot defaults,
            IRankProgression rankProgression,
            RankUpCatalog rankUps)
        {
            _defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _rankUps = rankUps ?? throw new ArgumentNullException(nameof(rankUps));
        }

        internal bool TryReconcile(
            UserSnapshot snapshot,
            out UserSnapshot reconciled,
            out bool changed)
        {
            reconciled = null;
            changed = false;

            if (snapshot is null || IsProgressCompatible(snapshot.Progress) is false)
                return false;

            var items = ReconcileItems(snapshot.Items, out var itemsChanged);
            var rankUpQuest = ReconcileRankUpQuest(
                snapshot.Progress,
                snapshot.RankUpQuest,
                out var rankUpQuestChanged);

            changed = itemsChanged || rankUpQuestChanged;
            reconciled = changed
                ? new UserSnapshot(snapshot.Identity, items, snapshot.Progress, rankUpQuest)
                : snapshot;
            return true;
        }

        private bool IsProgressCompatible(UserProgressSnapshot progress)
        {
            try
            {
                _rankProgression.GetProgress(progress.RankId, progress.Experience);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
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
                rankUpQuest.DeadlineUnixMilliseconds <= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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