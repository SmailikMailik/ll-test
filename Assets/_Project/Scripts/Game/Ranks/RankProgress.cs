using System;

namespace LL.Game.Ranks
{
    internal readonly struct RankProgress
    {
        internal int Rank { get; }
        internal int Experience { get; }
        internal int RequiredExperience { get; }
        internal int RemainingExperience { get; }
        internal bool HasNextRank { get; }

        internal RankProgress(
            int rank,
            int experience,
            int requiredExperience,
            bool hasNextRank)
        {
            Rank = rank;
            Experience = experience;
            RequiredExperience = requiredExperience;
            RemainingExperience = hasNextRank
                ? Math.Max(0, requiredExperience - experience)
                : 0;
            HasNextRank = hasNextRank;
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