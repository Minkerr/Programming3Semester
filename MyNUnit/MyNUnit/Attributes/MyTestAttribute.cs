namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MyTestAttribute : Attribute
{
    public Type? Expected { get; set; }
    public string? Ignore { get; set; }
    
    public MyTestAttribute()
    {
    }
}