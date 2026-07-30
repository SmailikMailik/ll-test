using System;
using VContainer;
using VContainer.Unity;

namespace LL.UI.Windows.Flows
{
    internal sealed class UpgradeFlowStartup : IStartable
    {
        private readonly UpgradeFlow _flow;

        [Inject]
        internal UpgradeFlowStartup(UpgradeFlow flow)
        {
            _flow = flow ?? throw new ArgumentNullException(nameof(flow));
        }

        public void Start() => _flow.Open();
    }
}