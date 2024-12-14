namespace ReceiptsApi.Web.CustomAttributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
public class OpenApiExampleAttribute : Attribute
{
  public string Example { get; }
  public OpenApiExampleAttribute(string example)
  {
    Example = example;
  }
}

