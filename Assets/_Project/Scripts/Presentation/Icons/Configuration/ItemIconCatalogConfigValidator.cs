using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Presentation.Icons.Configuration
{
    internal sealed class ItemIconCatalogConfigValidator : IDataValidator<ItemIconEntry[]>
    {
        private const string EntriesCode = "item-icon.entries.required";
        private const string IconCode = "item-icon.sprite.required";

        public void Validate(
            ItemIconEntry[] icons,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(icons, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                icons,
                entry => entry.Id,
                context);

            for (var index = 0; index < icons.Length; index++)
            {
                var entry = icons[index];

                if (entry is null)
                    continue;

                ValidationRules.NotNull(
                    entry.Icon,
                    context.At(index).At(nameof(ItemIconEntry.Icon)),
                    IconCode);
            }
        }
    }
}