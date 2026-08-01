namespace LL.Infrastructure.Loading
{
    internal interface IDataSource<out TDeclaration>
    {
        TDeclaration Read();
    }
}