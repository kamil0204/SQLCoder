using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using SQLCoderAPI.Services;
using System.Reflection;

namespace SQLCoderAPI.Extensions;

/// <summary>
/// Extension methods for registering Semantic Kernel related services.
/// </summary>
internal static class SemanticKernelExtensions
{
    /// <summary>
    /// Add Semantic Kernel services
    /// </summary>
    public static WebApplicationBuilder AddSemanticKernelServices(this WebApplicationBuilder builder)
    {
        builder.InitializeKernelProvider();

        // Semantic Kernel
        builder.Services.AddScoped<Kernel>(
            sp =>
            {
                var provider = sp.GetRequiredService<SemanticKernelProvider>();
                var kernel = provider.GetCompletionKernel();

                return kernel;
            });

        // Add any additional setup needed for the kernel.
        // Uncomment the following line and pass in a custom hook for any complimentary setup of the kernel.
        // builder.Services.AddKernelSetupHook(customHook);

        return builder;
    }

    private static void InitializeKernelProvider(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(sp => new SemanticKernelProvider(sp, builder.Configuration, sp.GetRequiredService<IHttpClientFactory>()));
    }
}

