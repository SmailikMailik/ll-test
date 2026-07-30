using System;

namespace LL.Game.Ranks
{
    internal readonly struct RankProgress
    {
        internal RankId RankId { get; }
        internal int Rank { get; }
        internal RankId NextRankId { get; }
        internal int Experience { get; }
        internal int RequiredExperience { get; }
        internal int RemainingExperience { get; }
        internal bool HasNextRank { get; }

        internal RankProgress(
            RankDefinition rank,
            RankDefinition nextRank,
            int experience)
        {
            if (rank == null)
                throw new ArgumentNullException(nameof(rank));

            RankId = rank.Id;
            Rank = rank.Number;
            NextRankId = nextRank?.Id ?? default;
            Experience = experience;
            RequiredExperience = nextRank?.RequiredExperience ?? 0;
            RemainingExperience = nextRank != null
                ? Math.Max(0, RequiredExperience - experience)
                : 0;
            HasNextRank = nextRank != null;
        }

        internal float GetNormalizedExperience(int experience)
        {
            return CalculateNormalizedExperience(experience, RequiredExperience, HasNextRank);
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