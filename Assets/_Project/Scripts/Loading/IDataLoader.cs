namespace LL.Loading
{
    internal interface IDataLoader<out T>
    {
        T Load();
    }
}