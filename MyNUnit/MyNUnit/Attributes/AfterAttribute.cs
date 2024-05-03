namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : Attribute
{
    public AfterAttribute()
    {
    }
}