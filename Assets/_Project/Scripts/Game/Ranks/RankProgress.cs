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
            NormalizedExperience = hasNextRank
                ? (float)totalExperience / nextRankExperience
                : 1f;
        }
    }
}