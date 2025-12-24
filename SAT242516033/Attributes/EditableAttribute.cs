using System;

namespace SAT242516033.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class EditableAttribute : Attribute
{
    public bool CanEdit { get; }
    public EditableAttribute(bool canEdit = true) => CanEdit = canEdit;
}
