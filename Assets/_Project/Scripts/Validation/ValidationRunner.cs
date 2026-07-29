using System;

namespace LL.Validation
{
    internal static class ValidationRunner
    {
        internal static ValidationResult Run(IValidationSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new ValidationResult();
            source.Validate(new ValidationContext(result));
            return result;
        }

        internal static ValidationResult Run<T>(
            T value,
            IDataValidator<T> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            var result = new ValidationResult();
            validator.Validate(value, new ValidationContext(result));
            return result;
        }

        internal static void EnsureValid(
            IValidationSource source,
            string parameterName = null)
        {
            ValidationResultGuard.EnsureValid(Run(source), parameterName);
        }

        internal static bool IsValid<T>(
            T value,
            IDataValidator<T> validator)
        {
            return Run(value, validator).IsValid;
        }
    }
}