namespace LL.Infrastructure.Saving.Storage
{
    internal interface ISaveStorage
    {
        bool Exists(string key);
        bool TryWrite(string key, byte[] data);
        bool TryRead(string key, out byte[] data);
        bool TryDelete(string key);
    }
}