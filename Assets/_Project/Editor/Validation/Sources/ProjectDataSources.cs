using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LLEditor.Validation.Sources
{
    internal sealed class ProjectDataSources
    {
        private readonly Dictionary<Type, ScriptableObject> _resolvedSources = new();
        private readonly IReadOnlyCollection<ScriptableObject> _sources;

        internal ProjectDataSources(IReadOnlyCollection<ScriptableObject> sources)
        {
            _sources = sources ?? throw new ArgumentNullException(nameof(sources));
        }

        internal TSource GetSingle<TSource>()
            where TSource : ScriptableObject
        {
            var sourceType = typeof(TSource);

            if (_resolvedSources.TryGetValue(sourceType, out var resolvedSource))
                return (TSource)resolvedSource;

            var matches = _sources
                .OfType<TSource>()
                .Take(2)
                .ToArray();

            var source = matches.Length == 1
                ? matches[0]
                : null;

            _resolvedSources.Add(sourceType, source);

            return source;
        }

        internal int Count(Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            return _sources.Count(sourceType.IsInstanceOfType);
        }
    }
}