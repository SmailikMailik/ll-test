namespace LL
{
    internal interface IDataLoader<out T>
    {
        T Load();
    }

    internal interface IDefaultDataLoader<out T> : IDataLoader<T> { }
}