using System;
using System.Linq;
using LL.Infrastructure.Loading;
using LL.UI.Windows.Configuration;
using LL.Validation;

namespace LL.UI.Windows.Loading
{
    internal sealed class ScriptableObjectWindowCatalogLoader : IDataLoader<WindowCatalog>
    {
        private readonly WindowCatalogConfig _config;

        internal ScriptableObjectWindowCatalogLoader(WindowCatalogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public WindowCatalog Load()
        {
            ValidationRunner.EnsureValid(_config, nameof(_config));

            return new WindowCatalog(_config.Entries.Select(entry => entry.ToDefinition()));
        }
    }
}