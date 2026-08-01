namespace LL.Infrastructure.Saving.Storage
{
    internal interface IReadOnlySaveStorage
    {
        bool Exists(string key);
        bool TryRead(string key, out byte[] data);
    }
}