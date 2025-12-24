using System;

namespace SAT242516033.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SortableAttribute : Attribute
{
    public bool Sortable { get; }
    public SortableAttribute(bool sortable = true) => Sortable = sortable;
}
