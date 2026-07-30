namespace LL.Bootstrap
{
    internal interface IBootstrapOperation
    {
        float Progress { get; }
        bool IsReady { get; }

        void Start();
        void EnsureSucceeded();
    }
}