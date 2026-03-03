using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Invoice_printer.extensions;

public static class EnumLocalizationExtensions
{
    public static string EnumKey<TEnum>(TEnum value) where TEnum : struct, Enum
        => $"{typeof(TEnum).Name}_{value}";

    public static string LocalizeEnum<TEnum>(this IStringLocalizer localizer, TEnum value)
        where TEnum : struct, Enum
    {
        var key = EnumKey(value);
        var s = localizer[key];
        // If not found, fall back to enum name
        return s.ResourceNotFound ? value.ToString() : s.Value;
    }

    public static IEnumerable<SelectListItem> ToLocalizedSelectList<TEnum>(
        this IStringLocalizer localizer,
        TEnum? selected = null,
        bool includeEmpty = false,
        string? emptyText = null
    ) where TEnum : struct, Enum
    {
        var items = new List<SelectListItem>();

        if (includeEmpty)
        {
            items.Add(new SelectListItem
            {
                Text = emptyText ?? "--",
                Value = "",
                Selected = selected == null
            });
        }

        foreach (var v in Enum.GetValues<TEnum>())
        {
            items.Add(new SelectListItem
            {
                Text = localizer.LocalizeEnum(v),
                Value = Convert.ToInt32(v).ToString(),
                Selected = selected != null && EqualityComparer<TEnum>.Default.Equals(v, selected.Value)
            });
        }

        return items;
    }
}

