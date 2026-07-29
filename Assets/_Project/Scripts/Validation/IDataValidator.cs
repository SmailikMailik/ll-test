namespace LL.Validation
{
    internal interface IDataValidator<in T>
    {
        void Validate(T value, ValidationContext context);
    }
}