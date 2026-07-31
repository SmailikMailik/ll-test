using System;

namespace LL.Validation
{
    internal static class ValidationRunner
    {
        internal static ValidationResult Run(Action<ValidationContext> validate)
        {
            if (validate == null)
                throw new ArgumentNullException(nameof(validate));

            var result = new ValidationResult();
            validate(new ValidationContext(result));
            return result;
        }

        internal static ValidationResult Run(IValidationSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new ValidationResult();
            source.Validate(new ValidationContext(result));
            return result;
        }

        internal static ValidationResult Run<T>(T value, IDataValidator<T> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            var result = new ValidationResult();
            validator.Validate(value, new ValidationContext(result));
            return result;
        }

        internal static void EnsureValid(IValidationSource source, string parameterName = null) =>
            ValidationResultGuard.EnsureValid(Run(source), parameterName);

        internal static void EnsureValid(Action<ValidationContext> validate, string parameterName = null) =>
            ValidationResultGuard.EnsureValid(Run(validate), parameterName);

        internal static void EnsureValid<T>(T value, IDataValidator<T> validator, string parameterName = null) =>
            ValidationResultGuard.EnsureValid(Run(value, validator), parameterName);

        internal static bool IsValid(IValidationSource source) =>
            Run(source).IsValid;

        internal static bool IsValid<T>(T value, IDataValidator<T> validator) =>
            Run(value, validator).IsValid;
    }
}