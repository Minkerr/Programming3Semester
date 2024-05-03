namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class BeforeAttribute : Attribute
{
    public BeforeAttribute()
    {
    }
}