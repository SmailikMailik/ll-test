namespace LL.UI.Windows.View.Upgrade
{
    internal sealed class UpgradeWindow : Window<UpgradeParameters> { }

    internal sealed class UpgradeParameters : IWindowParameters
    {
        internal int Id { get; }

        internal UpgradeParameters(int id)
        {
            Id = id;
        }
    }
}