namespace LL.User.Persistence
{
    internal enum UserReconciliationStatus : byte
    {
        Unchanged = 0,
        Changed = 1,
        Incompatible = 2
    }
}