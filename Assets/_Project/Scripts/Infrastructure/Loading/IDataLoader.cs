namespace LL.Infrastructure.Loading
{
    internal interface IDataLoader<out T>
    {
        T Load();
    }
}