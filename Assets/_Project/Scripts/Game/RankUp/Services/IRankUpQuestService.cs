namespace LL.Game.RankUp.Services
{
    internal interface IRankUpQuestService
    {
        bool TryStart(RankUpQuest quest);
        bool TryComplete();
        bool TryExpire();
        void Clear();
    }
}