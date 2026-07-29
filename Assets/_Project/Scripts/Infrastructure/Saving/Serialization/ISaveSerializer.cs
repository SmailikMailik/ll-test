namespace LL.Infrastructure.Saving.Serialization
{
    internal interface ISaveSerializer
    {
        bool TrySerialize<T>(T data, out byte[] bytes) where T : class;
        bool TryDeserialize<T>(byte[] bytes, out T data) where T : class;
    }
}