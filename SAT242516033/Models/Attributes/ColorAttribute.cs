namespace SAT242516033.Models.Attributes;

public class ColorAttribute(string color) : Attribute
{
    public string Color { get; set; } = color;
}