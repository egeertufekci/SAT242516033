namespace SAT242516033.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableAttribute : Attribute
    {
        public bool Value { get; set; }
        public EditableAttribute(bool value) => Value = value;
    }
}
