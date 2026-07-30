namespace LL.User.Persistence
{
    internal enum UserLoadStatus : byte
    {
        Loaded = 0,
        NotFound = 1,
        Corrupted = 2,
        UnsupportedVersion = 3
    }
}