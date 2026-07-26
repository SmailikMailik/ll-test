using System;

namespace LL.Game.Ranks
{
    internal readonly struct RankProgress
    {
        internal int Rank { get; }
        internal int TotalExperience { get; }

        internal int CurrentRankExperience { get; }
        internal int NextRankExperience { get; }

        internal int ExperienceInCurrentRank { get; }
        internal int ExperienceBetweenRanks { get; }

        internal float NormalizedExperience { get; }
        internal bool HasNextRank { get; }

        internal RankProgress(
            int rank,
            int totalExperience,
            int currentRankExperience,
            int nextRankExperience,
            bool hasNextRank)
        {
            Rank = rank;
            TotalExperience = totalExperience;

            CurrentRankExperience = currentRankExperience;
            NextRankExperience = nextRankExperience;

            ExperienceInCurrentRank = totalExperience - currentRankExperience;
            ExperienceBetweenRanks = hasNextRank
                ? nextRankExperience - currentRankExperience
                : 0;

            HasNextRank = hasNextRank;
            NormalizedExperience = CalculateNormalizedExperience(
                totalExperience,
                currentRankExperience,
                ExperienceBetweenRanks,
                hasNextRank);
        }

        internal float GetNormalizedExperience(int totalExperience)
        {
            return CalculateNormalizedExperience(
                totalExperience,
                CurrentRankExperience,
                ExperienceBetweenRanks,
                HasNextRank);
        }

        private static float CalculateNormalizedExperience(
            int totalExperience,
            int currentRankExperience,
            int experienceBetweenRanks,
            bool hasNextRank)
        {
            if (hasNextRank is false)
                return 1f;

            var experience = Math.Max(0, totalExperience - currentRankExperience);
            return (float)Math.Min(experience, experienceBetweenRanks) / experienceBetweenRanks;
        }
    }
}