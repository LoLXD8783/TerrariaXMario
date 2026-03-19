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
/// Generates a file containing <c>ModSystem</c> classes for <c>BetterUIState</c> classes with the <c>CanBeReadBySourceGeneratorsAttribute</c> attribute
/// </summary>

[Generator(LanguageNames.CSharp)]
public sealed class BetterUIStateSystemGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Stores names and namespaces of all classes that have the TerrariaXMario.CanBeReadBySourceGenerators attribute
        IncrementalValueProvider<ImmutableArray<ClassInfo>> collectedBetterUIStates =
            context.SyntaxProvider.ForAttributeWithMetadataName(
                "TerrariaXMario.CanBeReadBySourceGeneratorsAttribute",
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: (ctx, _) =>
                {
                    INamedTypeSymbol type = ctx.TargetSymbol as INamedTypeSymbol;

                    return GeneratorHelper.IsAssignableTo(type, "TerrariaXMario.Common.UIElements.BetterUIState", false) ? new ClassInfo(type.ContainingNamespace.ToString(), type.Name) : new ClassInfo("", "");
                }).Where(i => i.Name != "" && i.Namespace != "").Collect();

        context.RegisterSourceOutput(collectedBetterUIStates, (sourceProductionContext, uiStates) =>
        {
            string[] namespaces = [.. uiStates.Select(e => e.Namespace).Distinct()];

            StringBuilder text = new(2048);
            using IndentedTextWriter writer = new(new StringWriter(text));

            writer.WriteLine("using Terraria.UI;");
            foreach (string Namespace in namespaces)
            {
                writer.WriteLine($"namespace {Namespace}");
                writer.WriteLine("{");
                writer.Indent++;

                foreach (string uiState in uiStates.Where(e => e.Namespace == Namespace).Select(e => e.Name))
                {
                    string userInterface = $"{uiState}UserInterface";

                    writer.WriteLine("[Autoload(Side = ModSide.Client)]");
                    writer.WriteLine($"internal class {uiState}System : ModSystem");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("#nullable enable");
                    writer.WriteLine($"private UserInterface? {userInterface};");
                    writer.WriteLine($"private {uiState}? {uiState};");
                    writer.WriteLine("#nullable disable");
                    writer.WriteLine("public override void Load()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine($"{uiState} = new();");
                    writer.WriteLine($"{userInterface} = new();");
                    writer.WriteLine($"{userInterface}.SetState({uiState});");
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("public override void UpdateUI(GameTime gameTime)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine($"{userInterface}?.Update(gameTime);");
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("int index = layers.FindIndex(layer => layer.Name.Equals(\"Vanilla: Mouse Text\"));");
                    writer.WriteLine("if (index == -1) return;");
                    writer.WriteLine($"layers.Insert(index, new LegacyGameInterfaceLayer($\"{{nameof(TerrariaXMario)}}: {{{uiState}}}\", () =>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine($"if ({uiState}?.Visible ?? false) {userInterface}?.Draw(Main.spriteBatch, new());");
                    writer.WriteLine("return true;");
                    writer.Indent--;
                    writer.WriteLine($"}}, {uiState}?.InterfaceScaleType ?? InterfaceScaleType.UI));");
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.Indent--;
                    writer.WriteLine("}");
                }

                writer.Indent--;
                writer.WriteLine("}");
            }

            sourceProductionContext.AddSource("BetterUIStateSystems.g.cs", text.ToString());
        });
    }
}