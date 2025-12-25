namespace MyEnums
{
    public enum Operations
    {
        List,
        Add,
        Update,
        Remove,
        Detail,
        Cancel,
        Reset
    }

    public static class OperationsExtensions
    {
        // Mehmet'in kodundaki .Color() ve .Description() metodları için
        public static string Color(this Operations op) => op switch
        {
            Operations.Add => "success",
            Operations.Update => "warning",
            Operations.Remove => "danger",
            Operations.Detail => "info",
            Operations.List => "primary",
            _ => "secondary"
        };

        public static string Description(this Operations op) => op.ToString();
    }
}