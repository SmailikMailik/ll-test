using LL.Game.Heroes;

namespace LL.UI.Windows.Upgrade
{
    internal sealed class UpgradeWindowParameters : IWindowParameters
    {
        internal HeroId HeroId { get; }

        internal UpgradeWindowParameters(HeroId heroId)
        {
            HeroId = heroId;
        }
    }
}