namespace SAT242516033.Models.Attributes;

public class TitleAttribute(string title) : Attribute
{
    public string Title { get; set; } = title;
}