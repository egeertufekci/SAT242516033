using System;

namespace SAT242516033.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Class, AllowMultiple = false)]
public sealed class LocalizedDescriptionAttribute : Attribute
{
    public string Description { get; }

    public LocalizedDescriptionAttribute(string description)
    {
        Description = description;
    }
}
