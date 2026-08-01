using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.User.Defaults;
using LL.User.Snapshots;
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
            if (snapshot is null)
                return UserReconciliationResult.Incompatible();

            var heroes = ReconcileHeroes(snapshot.Heroes, out var heroesChanged);
            var heroSelection = ReconcileHeroSelection(snapshot.HeroSelection, heroes, out var heroSelectionChanged);
            var items = ReconcileItems(snapshot.Items, out var itemsChanged);

            if (heroesChanged is false && heroSelectionChanged is false && itemsChanged is false)
                return UserReconciliationResult.Unchanged(snapshot);

            return UserReconciliationResult.Changed(new UserSnapshot(snapshot.Identity, heroSelection, heroes, items));
        }

        private UserHeroesSnapshot ReconcileHeroes(
            UserHeroesSnapshot snapshot,
            out bool changed)
        {
            var defaultHeroes = _defaults.CreateUserSnapshot().Heroes;
            var reconciled = new List<UserHeroSnapshot>();
            var usedIds = new HashSet<HeroId>();
            changed = false;

            foreach (var hero in snapshot.Heroes)
            {
                if (_heroes.TryGetHero(hero.HeroId, out _) is false || usedIds.Add(hero.HeroId) is false)
                {
                    changed = true;
                    continue;
                }

                if (_rankProgression.TryGetProgress(hero.Progress.RankId, hero.Progress.Experience, out _) is false)
                {
                    if (defaultHeroes.TryGetHero(hero.HeroId, out var defaultHero))
                        reconciled.Add(defaultHero);

                    changed = true;
                    continue;
                }

                var attempts = ReconcileAttempts(hero, out var attemptsChanged);
                reconciled.Add(
                    attemptsChanged
                        ? new UserHeroSnapshot(hero.HeroId, hero.Progress, attempts)
                        : hero);
                changed |= attemptsChanged;
            }

            foreach (var defaultHero in defaultHeroes.Heroes)
            {
                if (usedIds.Add(defaultHero.HeroId))
                {
                    reconciled.Add(defaultHero);
                    changed = true;
                }
            }

            return changed ? new UserHeroesSnapshot(reconciled) : snapshot;
        }

        private IReadOnlyList<UserRankUpAttemptSnapshot> ReconcileAttempts(
            UserHeroSnapshot hero,
            out bool changed)
        {
            var attempts = new List<UserRankUpAttemptSnapshot>();
            changed = false;

            foreach (var attempt in hero.RankUpAttempts)
            {
                if (attempt.RankId.Equals(hero.Progress.RankId) is false ||
                    _rankUps.TryGetDefinition(hero.HeroId, attempt.RankId, out var definition) is false ||
                    definition.TryGetOption(attempt.OptionId, out var option) is false)
                {
                    changed = true;
                    continue;
                }

                var quests = ReconcileQuests(attempt, option, out var questsChanged);

                if (quests.Count == 0)
                {
                    changed = true;
                    continue;
                }

                attempts.Add(
                    questsChanged
                        ? new UserRankUpAttemptSnapshot(attempt.RankId, attempt.OptionId, quests)
                        : attempt);
                changed |= questsChanged;
            }

            return Array.AsReadOnly(attempts.ToArray());
        }

        private IReadOnlyList<UserRankUpQuestRequirementSnapshot> ReconcileQuests(
            UserRankUpAttemptSnapshot attempt,
            RankUpOptionDefinition option,
            out bool changed)
        {
            var quests = new List<UserRankUpQuestRequirementSnapshot>();
            var now = _timeProvider.GetUtcNow().ToUnixTimeMilliseconds();
            changed = false;

            foreach (var questState in attempt.Quests)
            {
                var definition = option.Requirements
                    .OfType<QuestRankUpRequirementDefinition>()
                    .FirstOrDefault(requirement => requirement.Id.Equals(questState.RequirementId));
                var expired = definition is not null &&
                              questState.CurrentCount < definition.RequiredCount &&
                              questState.DeadlineUnixMilliseconds <= now;

                if (definition is null || expired)
                {
                    changed = true;
                    continue;
                }

                quests.Add(questState);
            }

            return Array.AsReadOnly(quests.ToArray());
        }

        private UserHeroSelectionSnapshot ReconcileHeroSelection(
            UserHeroSelectionSnapshot heroSelection,
            UserHeroesSnapshot heroes,
            out bool changed)
        {
            changed = heroes.TryGetHero(heroSelection.HeroId, out _) is false;

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
    }
}