using LL.UI.StateRendering.States;
using NUnit.Framework;

namespace LL.Tests.EditMode.UI.StateRendering
{
    internal sealed class StateRenderingContractTests
    {
        [Test]
        public void StateEnums_DefineNormalAsInitialState()
        {
            Assert.That((int)InteractiveState.Normal, Is.Zero);
            Assert.That((int)SelectionState.Normal, Is.Zero);
        }
    }
}