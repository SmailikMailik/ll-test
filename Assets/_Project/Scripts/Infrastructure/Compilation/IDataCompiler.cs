namespace LL.Infrastructure.Compilation
{
    internal interface IDataCompiler<in TDeclaration, out TSnapshot>
    {
        TSnapshot Compile(TDeclaration declaration);
    }
}