namespace LL.Saving
{
    internal interface ISaveService
    {
        bool Exists(string key);
        bool TrySave<T>(string key, T data) where T : class;
        bool TryLoad<T>(string key, out T data) where T : class;
        bool TryDelete(string key);
    }
}