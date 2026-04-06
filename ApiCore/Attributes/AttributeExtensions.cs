namespace ApiCore.Attributes;

public static class AttributeExtensions
{
    public static void AddCustomAttributes(this IServiceCollection services)
    {
        _ = services.AddSingleton<InjectContextAttribute>();
        _ = services.AddSingleton<ValidateDuplicateRequestAttribute>();
    }
}