open BlazorWASMScriptLoader
open Fun.Blazor
open Microsoft.AspNetCore.Components.WebAssembly.Hosting
open Microsoft.CodeAnalysis
open Microsoft.Extensions.DependencyInjection
open Microsoft.JSInterop
open System.Threading.Tasks

#nowarn "0020"
let builder = WebAssemblyHostBuilder.CreateDefault()
builder.Services
    .AddSingleton<ScriptLoaderService>()
    .AddSingleton<MetadataReferenceService.BlazorWasm.BlazorWasmMetadataReferenceService>()
builder.AddFunBlazor("#app", html.inject(fun (js : IJSRuntime, sv : ScriptLoaderService) -> task {
    let! data = js.GetValueAsync<string>("window.frameElement.lastChild.data")
    let! asm = sv.CompileToDLLAssembly(data, "", true, SourceCodeKind.Script)
    let! result = asm.GetType("Script").GetMethod("<Factory>").Invoke(null, [| [| null; null |] |]) :?> Task<obj>
    return pre { result.ToString() }
})).Build().RunAsync()