using System;
using LL.Infrastructure.Loading;
using VContainer;

namespace LL.Composition.Installers
{
    internal static class LoadedDataRegistrationExtensions
    {
        internal static void RegisterLoadedData<TData>(
            this IContainerBuilder builder,
            IDataLoader<TData> loader)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (loader == null)
                throw new ArgumentNullException(nameof(loader));

            builder.RegisterInstance(loader);
            builder.RegisterLoadedData<TData>();
        }

        internal static void RegisterLoadedData<TData>(this IContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Register(resolver => resolver.Resolve<IDataLoader<TData>>().Load(), Lifetime.Singleton);
        }

        internal static void RegisterSnapshotPart<TSnapshot, TData>(
            this IContainerBuilder builder,
            Func<TSnapshot, TData> selector)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            builder.Register(resolver => selector(resolver.Resolve<TSnapshot>()), Lifetime.Singleton);
        }
    }
}