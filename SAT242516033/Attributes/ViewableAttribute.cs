using System;

namespace SAT242516033.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ViewableAttribute : Attribute
{
    public bool Visible { get; }
    public ViewableAttribute(bool visible = true) => Visible = visible;
}
