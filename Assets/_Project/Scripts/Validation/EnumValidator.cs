using System;

namespace LL.Validation
{
    internal static class EnumValidator
    {
        private const string UndefinedCode = "enum.undefined";

        internal static ValidationResult Validate<TEnum>(TEnum value) where TEnum : struct, Enum =>
            ValidationRunner.Run(context => Validate(value, context));

        internal static void Validate<TEnum>(
            TEnum value,
            ValidationContext context)
            where TEnum : struct, Enum
        {
            ValidationRules.DefinedEnum(value, context, UndefinedCode);
        }

        internal static bool IsValid<TEnum>(TEnum value) where TEnum : struct, Enum =>
            ValidationRunner.IsValid(context => Validate(value, context));

        internal static void EnsureValid<TEnum>(
            TEnum value,
            string parameterName)
            where TEnum : struct, Enum =>
            ValidationRunner.EnsureValid(context => Validate(value, context), parameterName);
    }
}