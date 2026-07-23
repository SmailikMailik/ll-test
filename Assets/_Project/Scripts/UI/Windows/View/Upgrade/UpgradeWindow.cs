using LL.UI.Windows.Core;

namespace LL.UI.Windows.View.Upgrade
{
    internal sealed class UpgradeWindow : WindowParameterized<UpgradeParameters> { }

    internal sealed class UpgradeParameters : IWindowParameters
    {
        internal int Id { get; }

        internal UpgradeParameters(int id)
        {
            Id = id;
        }
    }
}