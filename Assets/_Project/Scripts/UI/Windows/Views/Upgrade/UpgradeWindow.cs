namespace LL.UI.Windows.Views.Upgrade
{
    internal sealed class UpgradeWindow : Window<UpgradeWindowParameters> { }

    internal sealed class UpgradeWindowParameters : IWindowParameters
    {
        internal int Id { get; }

        internal UpgradeWindowParameters(int id)
        {
            Id = id;
        }
    }
}