using System;
using System.Collections.Generic;

namespace LL.UI.Windows
{
    internal sealed class WindowCatalog
    {
        private readonly IReadOnlyDictionary<Type, WindowDefinition> _definitions;

        internal WindowCatalog(IEnumerable<WindowDefinition> definitions)
        {
            if (definitions is null)
                throw new ArgumentNullException(nameof(definitions));

            var definitionsByParameterType = new Dictionary<Type, WindowDefinition>();

            foreach (var definition in definitions)
            {
                if (definition is null)
                    throw new ArgumentException("Window definition cannot be null.", nameof(definitions));

                if (definitionsByParameterType.TryAdd(definition.ParameterType, definition) is false)
                {
                    throw new ArgumentException(
                        $"Window parameter type '{definition.ParameterType.Name}' must be unique.",
                        nameof(definitions));
                }
            }

            _definitions = definitionsByParameterType;
        }

        internal bool TryGetDefinition(Type parameterType, out WindowDefinition definition)
        {
            if (parameterType is null)
                throw new ArgumentNullException(nameof(parameterType));

            return _definitions.TryGetValue(parameterType, out definition);
        }
    }
}