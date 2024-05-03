namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class BeforeClassAttribute : Attribute
{
    public BeforeClassAttribute()
    {
    }
}