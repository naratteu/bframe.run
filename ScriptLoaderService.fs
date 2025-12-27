module BlazorWASMScriptLoader

open MetadataReferenceService.Abstractions.Types
open MetadataReferenceService.BlazorWasm
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.Text
open System
open System.IO
open System.Reflection
open System.Threading.Tasks

type ScriptLoaderService(BlazorWasmMetadataReferenceService: BlazorWasmMetadataReferenceService) =
    member _.CompileToDLLAssembly(code: string, kind: SourceCodeKind) = task {
        let syntaxTree = SyntaxFactory.ParseSyntaxTree(
            code |> SourceText.From,
            kind |> CSharpParseOptions.Default.WithKind)
        let! refs =
            seq {
                yield! Assembly.GetEntryAssembly().GetReferencedAssemblies() |> Seq.map Assembly.Load
                yield! seq { typeof<obj>; typeof<Console> } |> Seq.map _.Assembly
            }
            |> Seq.map (fun a -> AssemblyDetails.FromAssembly(a))
            |> Seq.map (fun a -> BlazorWasmMetadataReferenceService.CreateAsync(a))
            |> Task.WhenAll
        let name = Path.GetRandomFileName()
        let compilation =
            match kind with
                | SourceCodeKind.Regular -> CSharpCompilation.Create(name, [ syntaxTree ], refs)
                | _ -> CSharpCompilation.CreateScriptCompilation(name, syntaxTree, refs)
        use ms = new MemoryStream()
        let result = ms |> compilation.Emit
        if not result.Success then
            result.Diagnostics
            |> Seq.filter (fun d -> d.IsWarningAsError || d.Severity = DiagnosticSeverity.Error)
            |> Seq.map (fun d ->
                let pos = d.Location.GetLineSpan().StartLinePosition
                $"Line: {pos.Line} Col:{pos.Character} Code: {d.Id} Message: {d.GetMessage()}")
            |> String.concat Environment.NewLine
            |> Exception
            |> raise
        ms.Position <- 0
        return ms.ToArray() |> Assembly.Load
    }