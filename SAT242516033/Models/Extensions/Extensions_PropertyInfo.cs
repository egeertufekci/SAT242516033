using System.Reflection;
using SAT242516033.Models.Attributes;

namespace SAT242516033.Models.Extensions
{
    public static class Extensions_PropertyInfo
    {
        public static bool Editable(this PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<SAT242516033.Models.Attributes.EditableAttribute>();
            return attr != null && attr.Value;
        }

        public static bool Sortable(this PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<SAT242516033.Models.Attributes.SortableAttribute>();
            return attr != null && attr.Value;
        }

        public static bool Viewable(this PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<SAT242516033.Models.Attributes.ViewableAttribute>();
            return attr != null && attr.Value;
        }

        public static string LocalizedDescription(this PropertyInfo propertyInfo)
        {
            try
            {
                var attr = propertyInfo.GetCustomAttribute<SAT242516033.Models.Attributes.LocalizedDescriptionAttribute>();
                return attr != null ? attr.Description : propertyInfo.Name;
            }
            catch
            {
                return propertyInfo.Name;
            }
        }
    }
}