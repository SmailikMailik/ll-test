using System;
using LL.Infrastructure.Compilation;
using LL.Infrastructure.Loading;
using LL.User.Defaults.Declarations;
using LL.User.Defaults.Sources;
using VContainer;

namespace LL.User.Defaults
{
    internal sealed class UserDefaultsLoader : IDataLoader<UserDefaultsSnapshot>
    {
        private readonly IUserDefaultsSource _source;
        private readonly IDataCompiler<UserDefaultsDeclaration, UserDefaultsSnapshot> _compiler;

        [Inject]
        internal UserDefaultsLoader(
            IUserDefaultsSource source,
            IDataCompiler<UserDefaultsDeclaration, UserDefaultsSnapshot> compiler)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        }

        public UserDefaultsSnapshot Load() => _compiler.Compile(_source.Read());
    }
}