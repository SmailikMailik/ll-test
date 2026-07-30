using LL.Game.Data.Declarations;

namespace LL.Game.Data
{
    internal interface IGameDataSource
    {
        GameDataDeclaration Read();
    }
}