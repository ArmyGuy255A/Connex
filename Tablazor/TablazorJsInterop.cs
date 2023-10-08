using Microsoft.JSInterop;

namespace Tablazor;

public class TablazorJsInterop
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public TablazorJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Tablazor/js/interop.js").AsTask());
    }
    
    public async ValueTask InitializeTooltips()
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("initializeTooltips");
    }
    
    public async ValueTask InitializeDropdowns()
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("initializeDropdowns");
    }
    
    public async ValueTask<string> Prompt(string message)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("showPrompt", message);
    }

    public async ValueTask Print()
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("print");
    }
    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}