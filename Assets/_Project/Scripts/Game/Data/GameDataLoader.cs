using System;
using LL.Game.Data.Declarations;
using LL.Infrastructure.Compilation;
using LL.Infrastructure.Loading;

namespace LL.Game.Data
{
    internal sealed class GameDataLoader : IDataLoader<GameDataSnapshot>
    {
        private readonly IGameDataSource _source;
        private readonly IDataCompiler<GameDataDeclaration, GameDataSnapshot> _compiler;

        internal GameDataLoader(
            IGameDataSource source,
            IDataCompiler<GameDataDeclaration, GameDataSnapshot> compiler)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        }

        public GameDataSnapshot Load() => _compiler.Compile(_source.Read());
    }
}