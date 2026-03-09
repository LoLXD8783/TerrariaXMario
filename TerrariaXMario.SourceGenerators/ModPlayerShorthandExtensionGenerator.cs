using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

namespace TerrariaXMario.SourceGenerators;
/// <summary>
/// Generates a file containing <c>Player</c> extension members for <c>ModPlayer</c> classes with the <c>CanBeReadBySourceGeneratorsAttribute</c> attribute
/// </summary>
/// <remarks>
/// The extension members generated act as shorthands for referencing a <c>ModPlayer</c> of an instanced <c>Player</c>. For each <c>ModPlayer</c> read by this generator, <c>player.GetModPlayer{ModPlayerClassName}()</c> can now be referenced by typing just <c>player.ModPlayerClassName</c>.
/// </remarks>

[Generator(LanguageNames.CSharp)]
public sealed class ModPlayerShorthandExtensionGenerator : IIncrementalGenerator
{
    // Used to store required type information
    private record struct ClassInfo(string Namespace, string Name);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Stores names and namespaces of all classes that have the TerrariaXMario.CanBeReadBySourceGenerators attribute
        IncrementalValueProvider<ImmutableArray<ClassInfo>> collectedPlayerInfos = context.SyntaxProvider.ForAttributeWithMetadataName("TerrariaXMario.CanBeReadBySourceGeneratorsAttribute",
            predicate: (node, cancelToken) => node is ClassDeclarationSyntax,
            transform: (context, cancellationToken) =>
            {
                ISymbol symbol = context.TargetSymbol;
                return new ClassInfo(symbol.ContainingNamespace.ToString(), symbol.Name.ToString());
            }).Collect();

        context.RegisterSourceOutput(collectedPlayerInfos, (sourceProductionContext, playerInfos) =>
        {
            string[] namespaces = [.. playerInfos.Select(e => e.Namespace).Distinct()];

            StringBuilder text = new(2048);
            using IndentedTextWriter writer = new(new StringWriter(text));

            foreach (string Namespace in namespaces)
            {
                writer.WriteLine($"using {Namespace};");
            }

            writer.WriteLine("namespace TerrariaXMario.Utilities.Extensions;");
            writer.WriteLine("internal static partial class PlayerExtensions");
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine("extension(Player player)");
            writer.WriteLine("{");
            writer.Indent++;

            foreach (ClassInfo playerClass in playerInfos)
            {
                writer.WriteLine($"internal {playerClass.Name} {playerClass.Name} => player.GetModPlayer<{playerClass.Name}>();");
            }

            writer.Indent--;
            writer.WriteLine("}");
            writer.Indent--;
            writer.WriteLine("}");

            sourceProductionContext.AddSource("ModPlayerShorthandExtension.g.cs", text.ToString());
        });
    }
}