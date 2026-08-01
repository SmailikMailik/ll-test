using LL.Game.Heroes;

namespace LL.UI.Windows.RankUp
{
    internal sealed class RankUpWindowParameters : IWindowParameters
    {
        internal HeroId HeroId { get; }

        internal RankUpWindowParameters(HeroId heroId)
        {
            HeroId = heroId;
        }
    }
}