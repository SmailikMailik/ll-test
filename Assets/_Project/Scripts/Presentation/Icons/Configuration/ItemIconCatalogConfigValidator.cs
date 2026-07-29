using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Presentation.Icons.Configuration
{
    internal sealed class ItemIconCatalogConfigValidator : IDataValidator<ItemIconEntry[]>
    {
        private const string IconCode = "item-icon.sprite.required";

        public void Validate(
            ItemIconEntry[] icons,
            ValidationContext context)
        {
            IdentifierCollectionValidator.Validate(
                icons,
                entry => entry.Id,
                context);

            if (icons == null)
                return;

            for (var index = 0; index < icons.Length; index++)
            {
                var entry = icons[index];

                if (entry == null)
                    continue;

                ValidationRules.NotNull(
                    entry.Icon,
                    context.At(index).At("Icon"),
                    IconCode);
            }
        }
    }
}