namespace LL.Game.Identifiers
{
    internal interface IIdentifier
    {
        string Value { get; }
        bool IsEmpty { get; }
    }
}