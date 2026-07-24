namespace LL
{
    internal sealed class UserData
    {
        internal IReadOnlyReactiveParameter<int> SoftAmount => _softAmount;
        internal IReadOnlyReactiveParameter<int> HardAmount => _hardAmount;
        internal IReadOnlyReactiveParameter<int> MasterPointAmount => _masterPointAmount;

        private readonly ReactiveParameter<int> _softAmount;
        private readonly ReactiveParameter<int> _hardAmount;
        private readonly ReactiveParameter<int> _masterPointAmount;

        internal UserData(
            int defaultSoftAmount,
            int defaultHardAmount,
            int defaultMasterPointAmount)
        {
            _softAmount = new ReactiveParameter<int>(defaultSoftAmount);
            _hardAmount = new ReactiveParameter<int>(defaultHardAmount);
            _masterPointAmount = new ReactiveParameter<int>(defaultMasterPointAmount);
        }
    }
}