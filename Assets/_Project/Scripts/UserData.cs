namespace LL
{
    internal sealed class UserData
    {
        internal int SoftAmount { get; private set; }
        internal int HardAmount { get; private set; }

        internal void Init()
        {
            SoftAmount = 5;
            HardAmount = 10;
        }
    }
}