namespace LL.Infrastructure.Saving.Storage
{
    internal interface ISaveStorage : IReadOnlySaveStorage
    {
        bool TryWrite(string key, byte[] data);
        bool TryDelete(string key);
    }
}