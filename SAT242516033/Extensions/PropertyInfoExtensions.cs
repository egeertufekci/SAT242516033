using System.Linq;
using System.Reflection;
using SAT242516033.Attributes;

namespace SAT242516033.Extensions;

public static class PropertyInfoExtensions
{
    public static string? GetLocalizedDescription(this PropertyInfo prop)
    {
        var attr = prop.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), true).FirstOrDefault() as LocalizedDescriptionAttribute;
        return attr?.Description;
    }
    public static bool IsEditable(this PropertyInfo prop)
    {
        var attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
        return attr?.CanEdit ?? true;
    }

    public static bool IsViewable(this PropertyInfo prop)
    {
        var attr = prop.GetCustomAttributes(typeof(ViewableAttribute), true).FirstOrDefault() as ViewableAttribute;
        return attr?.Visible ?? true;
    }

    public static bool IsSortable(this PropertyInfo prop)
    {
        var attr = prop.GetCustomAttributes(typeof(SortableAttribute), true).FirstOrDefault() as SortableAttribute;
        return attr?.Sortable ?? false;
    }
}
