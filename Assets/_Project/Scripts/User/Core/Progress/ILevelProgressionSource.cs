namespace LL.User.Core.Progress
{
    internal interface ILevelProgressionSource
    {
        LevelProgressionData Load();
    }
}