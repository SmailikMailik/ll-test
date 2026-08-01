using LL.Infrastructure.Compilation;
using LL.Infrastructure.Loading;
using NUnit.Framework;

namespace LL.Tests.EditMode
{
    internal sealed class CompiledDataLoaderTests
    {
        [Test]
        public void LoadPassesSourceDeclarationToCompiler()
        {
            var loader = new CompiledDataLoader<int, string>(
                new StubSource(7),
                new StubCompiler());

            Assert.That(loader.Load(), Is.EqualTo("7"));
        }

        private sealed class StubSource : IDataSource<int>
        {
            private readonly int _value;

            internal StubSource(int value)
            {
                _value = value;
            }

            public int Read() => _value;
        }

        private sealed class StubCompiler : IDataCompiler<int, string>
        {
            public string Compile(int declaration) => declaration.ToString();
        }
    }
}