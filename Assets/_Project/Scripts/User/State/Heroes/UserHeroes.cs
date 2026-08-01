using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.User.Snapshots;
using LL.User.State.RankUp;
using R3;
using VContainer;

namespace LL.User.State.Heroes
{
    internal sealed class UserHeroes :
        IUserHeroProgress,
        IUserHeroProgressCommands,
        IUserRankUpAttempts,
        IUserRankUpAttemptsCommands,
        IDisposable
    {
        public Observable<Unit> Changed => _changed;

        private readonly Subject<Unit> _changed = new();
        private readonly Dictionary<HeroId, HeroState> _heroes;
        private readonly IRankProgression _rankProgression;
        private readonly TimeProvider _timeProvider;

        [Inject]
        internal UserHeroes(
            UserHeroesSnapshot snapshot,
            IRankProgression rankProgression,
            TimeProvider timeProvider)
        {
            if (snapshot is null)
                throw new ArgumentNullException(nameof(snapshot));

            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            _heroes = snapshot.Heroes.ToDictionary(hero => hero.HeroId, ToHeroState);
        }

        public bool TryGetProgress(HeroId heroId, out UserProgressSnapshot progress)
        {
            if (_heroes.TryGetValue(heroId, out var hero) is false)
            {
                progress = null;
                return false;
            }

            progress = new UserProgressSnapshot(hero.RankId, hero.Experience);
            return true;
        }

        public int GetRankNumber(HeroId heroId)
        {
            return TryGetHeroProgress(heroId, out var progress) ? progress.RankNumber : 0;
        }

        public int GetApplicableExperience(HeroId heroId, int amount)
        {
            if (amount <= 0 || TryGetHeroProgress(heroId, out var progress) is false)
                return 0;

            return Math.Min(amount, progress.RemainingExperience);
        }

        public bool CanAddExperience(HeroId heroId, int amount) =>
            GetApplicableExperience(heroId, amount) > 0;

        public bool CanRankUp(HeroId heroId) =>
            _heroes.TryGetValue(heroId, out var hero) &&
            _rankProgression.CanRankUp(hero.RankId, hero.Experience);

        public bool TryAddExperience(HeroId heroId, int amount)
        {
            var appliedExperience = GetApplicableExperience(heroId, amount);

            if (appliedExperience <= 0 || _heroes.TryGetValue(heroId, out var hero) is false)
                return false;

            hero.Experience += appliedExperience;
            NotifyChanged();
            return true;
        }

        public bool TryRankUp(HeroId heroId)
        {
            if (_heroes.TryGetValue(heroId, out var hero) is false ||
                _rankProgression.CanRankUp(hero.RankId, hero.Experience) is false)
            {
                return false;
            }

            var previousRankId = hero.RankId;
            var progress = _rankProgression.GetProgress(hero.RankId, hero.Experience);
            var nextProgress = _rankProgression.GetProgress(progress.NextRankId, 0);
            hero.RankId = nextProgress.RankId;
            hero.Experience = nextProgress.Experience;
            hero.Attempts.RemoveAll(attempt => attempt.RankId.Equals(previousRankId));
            NotifyChanged();
            return true;
        }

        public bool TryGetQuest(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementId requirementId,
            out UserRankUpQuestRequirementSnapshot quest)
        {
            quest = null;

            if (TryGetAttempt(heroId, rankId, optionId, out var attempt) is false)
                return false;

            var state = attempt.Quests.FirstOrDefault(value => value.RequirementId.Equals(requirementId));

            if (state is null)
                return false;

            quest = state.CreateSnapshot();
            return true;
        }

        public TimeSpan GetRemainingTime(UserRankUpQuestRequirementSnapshot quest)
        {
            if (quest is null)
                throw new ArgumentNullException(nameof(quest));

            var remainingMilliseconds = quest.DeadlineUnixMilliseconds - _timeProvider.GetUtcNow().ToUnixTimeMilliseconds();
            return TimeSpan.FromMilliseconds(Math.Max(0L, remainingMilliseconds));
        }

        public bool TryStartQuest(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementId requirementId,
            TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero ||
                _heroes.TryGetValue(heroId, out var hero) is false ||
                hero.RankId.Equals(rankId) is false)
            {
                return false;
            }

            var attempt = GetOrCreateAttempt(hero, rankId, optionId);

            if (attempt.Quests.Any(quest => quest.RequirementId.Equals(requirementId)))
                return false;

            var deadline = _timeProvider.GetUtcNow().Add(duration).ToUnixTimeMilliseconds();
            attempt.Quests.Add(new QuestState(requirementId, 0, deadline));
            NotifyChanged();
            return true;
        }

        public bool TryAddQuestProgress(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementId requirementId,
            int amount)
        {
            if (amount <= 0 || TryGetAttempt(heroId, rankId, optionId, out var attempt) is false)
                return false;

            var quest = attempt.Quests.FirstOrDefault(value => value.RequirementId.Equals(requirementId));

            if (quest is null || quest.DeadlineUnixMilliseconds <= _timeProvider.GetUtcNow().ToUnixTimeMilliseconds())
                return false;

            var nextCount = Math.Min(int.MaxValue, (long)quest.CurrentCount + amount);
            quest.CurrentCount = (int)nextCount;
            NotifyChanged();
            return true;
        }

        public bool TryExpireQuest(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementId requirementId)
        {
            if (TryGetAttempt(heroId, rankId, optionId, out var attempt) is false)
                return false;

            var removed = attempt.Quests.RemoveAll(
                quest => quest.RequirementId.Equals(requirementId) &&
                         quest.DeadlineUnixMilliseconds <= _timeProvider.GetUtcNow().ToUnixTimeMilliseconds()) > 0;

            if (removed is false)
                return false;

            RemoveAttemptIfEmpty(heroId, attempt);
            NotifyChanged();
            return true;
        }

        public void ClearOption(HeroId heroId, RankId rankId, RankUpOptionId optionId)
        {
            if (_heroes.TryGetValue(heroId, out var hero) is false)
                return;

            if (hero.Attempts.RemoveAll(
                    attempt => attempt.RankId.Equals(rankId) && attempt.OptionId.Equals(optionId)) == 0)
            {
                return;
            }

            NotifyChanged();
        }

        public void ClearRank(HeroId heroId, RankId rankId)
        {
            if (_heroes.TryGetValue(heroId, out var hero) is false ||
                hero.Attempts.RemoveAll(attempt => attempt.RankId.Equals(rankId)) == 0)
            {
                return;
            }

            NotifyChanged();
        }

        internal UserHeroesSnapshot CreateSnapshot()
        {
            return new UserHeroesSnapshot(_heroes.Values.Select(ToUserHeroSnapshot));
        }

        public void Dispose()
        {
            _changed.Dispose();
        }

        private bool TryGetHeroProgress(HeroId heroId, out RankProgress progress)
        {
            if (_heroes.TryGetValue(heroId, out var hero) is false)
            {
                progress = default;
                return false;
            }

            progress = _rankProgression.GetProgress(hero.RankId, hero.Experience);
            return true;
        }

        private bool TryGetAttempt(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            out AttemptState attempt)
        {
            attempt = null;

            if (_heroes.TryGetValue(heroId, out var hero) is false)
                return false;

            attempt = hero.Attempts.FirstOrDefault(
                value => value.RankId.Equals(rankId) && value.OptionId.Equals(optionId));
            return attempt is not null;
        }

        private static AttemptState GetOrCreateAttempt(
            HeroState hero,
            RankId rankId,
            RankUpOptionId optionId)
        {
            var attempt = hero.Attempts.FirstOrDefault(
                value => value.RankId.Equals(rankId) && value.OptionId.Equals(optionId));

            if (attempt is not null)
                return attempt;

            attempt = new AttemptState(rankId, optionId, new List<QuestState>());
            hero.Attempts.Add(attempt);
            return attempt;
        }

        private void RemoveAttemptIfEmpty(HeroId heroId, AttemptState attempt)
        {
            if (attempt.Quests.Count == 0 && _heroes.TryGetValue(heroId, out var hero))
                hero.Attempts.Remove(attempt);
        }

        private HeroState ToHeroState(UserHeroSnapshot hero)
        {
            var progress = _rankProgression.GetProgress(hero.Progress.RankId, hero.Progress.Experience);
            return new HeroState(
                hero.HeroId,
                progress.RankId,
                progress.Experience,
                hero.RankUpAttempts.Select(ToAttemptState).ToList());
        }

        private static AttemptState ToAttemptState(UserRankUpAttemptSnapshot attempt)
        {
            return new AttemptState(
                attempt.RankId,
                attempt.OptionId,
                attempt.Quests.Select(ToQuestState).ToList());
        }

        private static QuestState ToQuestState(UserRankUpQuestRequirementSnapshot quest)
        {
            return new QuestState(
                quest.RequirementId,
                quest.CurrentCount,
                quest.DeadlineUnixMilliseconds);
        }

        private static UserHeroSnapshot ToUserHeroSnapshot(HeroState hero)
        {
            return new UserHeroSnapshot(
                hero.HeroId,
                new UserProgressSnapshot(hero.RankId, hero.Experience),
                hero.Attempts.Select(ToUserRankUpAttemptSnapshot));
        }

        private static UserRankUpAttemptSnapshot ToUserRankUpAttemptSnapshot(AttemptState attempt)
        {
            return new UserRankUpAttemptSnapshot(
                attempt.RankId,
                attempt.OptionId,
                attempt.Quests.Select(quest => quest.CreateSnapshot()));
        }

        private void NotifyChanged()
        {
            _changed.OnNext(Unit.Default);
        }

        private sealed class HeroState
        {
            internal HeroId HeroId { get; }
            internal RankId RankId { get; set; }
            internal int Experience { get; set; }
            internal List<AttemptState> Attempts { get; }

            internal HeroState(
                HeroId heroId,
                RankId rankId,
                int experience,
                List<AttemptState> attempts)
            {
                HeroId = heroId;
                RankId = rankId;
                Experience = experience;
                Attempts = attempts;
            }
        }

        private sealed class AttemptState
        {
            internal RankId RankId { get; }
            internal RankUpOptionId OptionId { get; }
            internal List<QuestState> Quests { get; }

            internal AttemptState(
                RankId rankId,
                RankUpOptionId optionId,
                List<QuestState> quests)
            {
                RankId = rankId;
                OptionId = optionId;
                Quests = quests;
            }
        }

        private sealed class QuestState
        {
            internal RankUpRequirementId RequirementId { get; }
            internal int CurrentCount { get; set; }
            internal long DeadlineUnixMilliseconds { get; }

            internal QuestState(
                RankUpRequirementId requirementId,
                int currentCount,
                long deadlineUnixMilliseconds)
            {
                RequirementId = requirementId;
                CurrentCount = currentCount;
                DeadlineUnixMilliseconds = deadlineUnixMilliseconds;
            }

            internal UserRankUpQuestRequirementSnapshot CreateSnapshot()
            {
                return new UserRankUpQuestRequirementSnapshot(
                    RequirementId,
                    CurrentCount,
                    DeadlineUnixMilliseconds);
            }
        }
    }
}