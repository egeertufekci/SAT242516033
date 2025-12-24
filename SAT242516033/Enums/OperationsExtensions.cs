using System.ComponentModel;
using System.Reflection;

namespace SAT242516033.Enums;

public static class OperationsExtensions
{
    public static string Description(this Operations op)
    {
        var mem = typeof(Operations).GetMember(op.ToString()).FirstOrDefault();
        var attr = mem?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? op.ToString();
    }
}
