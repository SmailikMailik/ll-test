namespace LL.Infrastructure.Saving
{
    internal interface IReadOnlySaveService
    {
        bool Exists(string key);
        bool TryLoad<T>(string key, out T data) where T : class;
    }
}