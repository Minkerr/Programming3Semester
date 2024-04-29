namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AfterClassAttribute : Attribute
{
    public AfterClassAttribute()
    {
    }
}