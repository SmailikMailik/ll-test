using System;

namespace LL.Game.Ranks
{
    internal readonly struct RankProgress
    {
        internal RankId RankId { get; }
        internal int RankNumber { get; }
        internal RankId NextRankId { get; }
        internal int Experience { get; }
        internal int ExperienceRequiredForRankUp { get; }
        internal int RemainingExperience { get; }
        internal bool HasNextRank { get; }

        internal RankProgress(
            RankDefinition rank,
            RankDefinition nextRank,
            int experience)
        {
            if (rank is null)
                throw new ArgumentNullException(nameof(rank));

            RankId = rank.Id;
            RankNumber = rank.Number;
            NextRankId = nextRank?.Id ?? default;
            Experience = experience;
            ExperienceRequiredForRankUp = nextRank?.RequiredExperience ?? 0;
            RemainingExperience = nextRank is not null
                ? Math.Max(0, ExperienceRequiredForRankUp - experience)
                : 0;
            HasNextRank = nextRank is not null;
        }

        internal float GetNormalizedExperience(int experience)
        {
            return CalculateNormalizedExperience(experience, ExperienceRequiredForRankUp, HasNextRank);
        }

        private static float CalculateNormalizedExperience(
            int experience,
            int requiredExperience,
            bool hasNextRank)
        {
            if (hasNextRank is false)
                return 1f;

            return (float)Math.Clamp(experience, 0, requiredExperience) / requiredExperience;
        }
    }
}