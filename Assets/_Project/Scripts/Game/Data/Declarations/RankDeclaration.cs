namespace LL.Game.Data.Declarations
{
    internal sealed class RankDeclaration
    {
        internal string Id { get; }
        internal int RequiredExperience { get; }

        internal RankDeclaration(string id, int requiredExperience)
        {
            Id = id;
            RequiredExperience = requiredExperience;
        }
    }
}