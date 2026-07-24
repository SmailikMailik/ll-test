using System;
using R3;

namespace LL
{
    internal sealed class UserData : IDisposable
    {
        internal Observable<int> SoftAmount => _softAmount;
        internal Observable<int> HardAmount => _hardAmount;
        internal Observable<int> MasterPointAmount => _masterPointAmount;

        private readonly ReactiveProperty<int> _softAmount;
        private readonly ReactiveProperty<int> _hardAmount;
        private readonly ReactiveProperty<int> _masterPointAmount;

        internal UserData(
            int defaultSoftAmount,
            int defaultHardAmount,
            int defaultMasterPointAmount)
        {
            _softAmount = new ReactiveProperty<int>(defaultSoftAmount);
            _hardAmount = new ReactiveProperty<int>(defaultHardAmount);
            _masterPointAmount = new ReactiveProperty<int>(defaultMasterPointAmount);
        }

        public void Dispose()
        {
            _softAmount.Dispose();
            _hardAmount.Dispose();
            _masterPointAmount.Dispose();
        }
    }
}