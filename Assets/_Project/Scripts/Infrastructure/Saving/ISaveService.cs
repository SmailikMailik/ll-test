namespace LL.Infrastructure.Saving
{
    internal interface ISaveService : IReadOnlySaveService
    {
        bool TrySave<T>(string key, T data) where T : class;
        bool TryDelete(string key);
    }
}