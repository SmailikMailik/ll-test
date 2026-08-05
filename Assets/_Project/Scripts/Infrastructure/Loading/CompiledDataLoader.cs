using System;
using LL.Infrastructure.Compilation;

namespace LL.Infrastructure.Loading
{
    internal sealed class CompiledDataLoader<TDeclaration, TSnapshot> : IDataLoader<TSnapshot>
    {
        private readonly IDataSource<TDeclaration> _source;
        private readonly IDataCompiler<TDeclaration, TSnapshot> _compiler;

        internal CompiledDataLoader(
            IDataSource<TDeclaration> source,
            IDataCompiler<TDeclaration, TSnapshot> compiler)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        }

        public TSnapshot Load() => _compiler.Compile(_source.Read());
    }
}