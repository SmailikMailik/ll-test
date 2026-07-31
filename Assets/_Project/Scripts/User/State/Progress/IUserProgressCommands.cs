namespace LL.User.State.Progress
{
    internal interface IUserProgressCommands
    {
        bool TryAddExperience(int amount);
        bool TryRankUp();
    }
}