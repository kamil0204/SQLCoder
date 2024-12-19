using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SQLCoderAPI.Services;

/// <summary>
/// Extension methods for registering Semantic Kernel related services.
/// </summary>
public sealed class SemanticKernelProvider
{
    private readonly Kernel _kernel;

    public SemanticKernelProvider(IServiceProvider serviceProvider, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        this._kernel = InitializeCompletionKernel(serviceProvider, configuration, httpClientFactory);
    }

    /// <summary>
    /// Produce semantic-kernel with only completion services for chat.
    /// </summary>
    public Kernel GetCompletionKernel() => this._kernel.Clone();

    private static Kernel InitializeCompletionKernel(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {

        var builder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.AddOllamaChatCompletion("mannix/defog-llama3-sqlcoder-8b", httpClientFactory.CreateClient("ollamaclient"));
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        //var chatCompletionService = serviceProvider.GetRequiredService<IChatCompletionService>();

        return builder.Build();
    }
}
    
