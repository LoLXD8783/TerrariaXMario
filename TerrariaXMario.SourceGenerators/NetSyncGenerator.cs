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
/// Generates a file containing the necessary synchronization code for any field with the <c>NetSyncAttribute</c>
/// </summary>
/// <remarks>
/// All read fields must be in partial classes. This generator creates an enum containing all necesarry NetMessage types, as well as all required synchronization code for each class.
/// </remarks>

[Generator(LanguageNames.CSharp)]
public sealed class NetSyncGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Stores relevant information about fields with the NetSyncAttribute
        IncrementalValueProvider<ImmutableArray<FieldInfo>> collectedFieldInfos = context.SyntaxProvider.ForAttributeWithMetadataName("TerrariaXMario.NetSyncAttribute",
            predicate: (node, cancelToken) => node is VariableDeclaratorSyntax || node is PropertyDeclarationSyntax,
            transform: (context, cancellationToken) =>
            {
                ISymbol symbol = context.TargetSymbol;
                INamedTypeSymbol container = symbol.ContainingType;

                ITypeSymbol type = symbol switch
                {
                    ILocalSymbol local => local.Type,
                    IFieldSymbol field => field.Type,
                    IPropertySymbol prop => prop.Type,
                    _ => null
                };

                string typeName = type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "unknown";

                bool shouldSave = false;

                foreach (var attr in symbol.GetAttributes())
                {
                    if (attr.AttributeClass?.ToDisplayString() == "TerrariaXMario.NetSyncAttribute")
                    {
                        foreach (var arg in attr.NamedArguments)
                        {
                            if (arg.Key == "ShouldSave" && arg.Value.Value is bool b) shouldSave = b;
                        }
                    }
                }

                return new FieldInfo(symbol.Name.ToString(), typeName, new ClassInfo(container.ContainingNamespace.ToString(), container.Name.ToString()), shouldSave);
            }).Collect();

        context.RegisterSourceOutput(collectedFieldInfos, (sourceProductionContext, fieldInfos) =>
        {
            ClassInfo[] containers = [.. fieldInfos.Select(e => e.Container).Distinct()];
            string[] containerNames = [.. containers.Select(e => e.Name)];
            bool shouldSave = fieldInfos.Any(e => (bool)e.ExtraData[0]);

            StringBuilder text = new(2048);
            using IndentedTextWriter writer = new(new StringWriter(text));

            if (shouldSave) writer.WriteLine("using Terraria.ModLoader.IO;");
            writer.WriteLine("using Terraria.ID;");
            writer.WriteLine("namespace TerrariaXMario");
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine("internal partial class TerrariaXMario : Mod");
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine("internal enum NetMessageType : byte");
            writer.WriteLine("{");
            writer.Indent++;

            foreach (string container in containerNames)
            {
                writer.WriteLine($"{container}Sync,");
            }

            writer.Indent--;
            writer.WriteLine("}");
            writer.WriteLine("public override void HandlePacket(BinaryReader reader, int whoAmI)");
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine("switch ((NetMessageType)reader.ReadByte())");
            writer.WriteLine("{");
            writer.Indent++;

            foreach (string container in containerNames)
            {
                writer.WriteLine($"case NetMessageType.{container}Sync:");
                writer.WriteLine("{");
                writer.Indent++;
                writer.WriteLine("Player player = Main.player[reader.ReadByte()];");
                writer.WriteLine($"player.{container}.ReceivePlayerSync(reader);");
                writer.WriteLine($"if (Main.netMode == NetmodeID.Server) player.{container}.SyncPlayer(-1, whoAmI, false);");
                writer.WriteLine("break;");
                writer.Indent--;
                writer.WriteLine("}");
            }

            writer.WriteLine("default:");
            writer.Indent++;
            writer.WriteLine("break;");
            writer.Indent--;
            writer.Indent--;
            writer.WriteLine("}");
            writer.Indent--;
            writer.WriteLine("}");
            writer.Indent--;
            writer.WriteLine("}");
            writer.Indent--;
            writer.WriteLine("}");

            foreach (ClassInfo container in containers)
            {
                string containerName = container.Name;
                FieldInfo[] fields = [.. fieldInfos.Where(e => e.Container.Name == containerName)];
                string[] fieldNames = [.. fields.Select(e => e.Name)];

                writer.WriteLine($"namespace {container.Namespace}");
                writer.WriteLine("{");
                writer.Indent++;
                writer.WriteLine($"internal partial class {containerName}");
                writer.WriteLine("{");
                writer.Indent++;


                writer.WriteLine("public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)");
                writer.WriteLine("{");
                writer.Indent++;
                writer.WriteLine("ModPacket packet = Mod.GetPacket();");
                writer.WriteLine($"packet.Write((byte)TerrariaXMario.NetMessageType.{containerName}Sync);");
                writer.WriteLine("packet.Write((byte)Player.whoAmI);");

                foreach (FieldInfo field in fields)
                {
                    if (field.Type == "global::Microsoft.Xna.Framework.Vector2") writer.WriteLine($"packet.WriteVector2({field.Name});");
                    else writer.WriteLine($"packet.Write({field.Name});");
                }

                writer.WriteLine("packet.Send(toWho, fromWho);");
                writer.Indent--;
                writer.WriteLine("}");
                writer.WriteLine("internal void ReceivePlayerSync(BinaryReader reader)");
                writer.WriteLine("{");
                writer.Indent++;

                foreach (FieldInfo field in fields)
                {
                    string readName = field.Type.Replace("global::", "") switch
                    {
                        "string" => "String",
                        "bool" => "Boolean",
                        "int" => "Int32",
                        "float" => "Single",
                        "Microsoft.Xna.Framework.Vector2" => "Vector2",
                        _ => throw new Exception($"Bad field type {field.Name} {field.Type} {field.Container.Name} {field.Container.Namespace}")
                    };

                    writer.WriteLine($"{field.Name} = reader.Read{readName}();");
                }

                writer.Indent--;
                writer.WriteLine("}");
                writer.WriteLine("public override void CopyClientState(ModPlayer targetCopy)");
                writer.WriteLine("{");
                writer.Indent++;
                writer.WriteLine($"{containerName} clone = ({containerName})targetCopy;");

                foreach (string field in fieldNames)
                {
                    writer.WriteLine($"clone.{field} = {field};");
                }

                writer.Indent--;
                writer.WriteLine("}");
                writer.WriteLine("public override void SendClientChanges(ModPlayer clientPlayer)");
                writer.WriteLine("{");
                writer.Indent++;
                writer.WriteLine($"{containerName} clone = ({containerName})clientPlayer;");
                writer.WriteLine("if (");

                foreach (string field in fieldNames)
                {
                    writer.WriteLine($"clone.{field} != {field} {(field == fieldNames.Last() ? "" : "||")}");
                }

                writer.WriteLine(") SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);");
                writer.Indent--;
                writer.WriteLine("}");

                if (fields.Any(e => (bool)e.ExtraData[0]))
                {
                    FieldInfo[] fieldsThatNeedSaving = [.. fields.Where(e => (bool)e.ExtraData[0])];

                    writer.WriteLine("public override void SaveData(TagCompound tag)");
                    writer.WriteLine("{");
                    writer.Indent++;

                    foreach (FieldInfo fieldThatNeedsSaving in fieldsThatNeedSaving)
                    {
                        writer.WriteLine($"tag[nameof({fieldThatNeedsSaving.Name})] = {fieldThatNeedsSaving.Name};");
                    }

                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("public override void LoadData(TagCompound tag)");
                    writer.WriteLine("{");
                    writer.Indent++;

                    foreach (FieldInfo fieldThatNeedsSaving in fieldsThatNeedSaving)
                    {
                        string readName = fieldThatNeedsSaving.Type switch
                        {
                            "string" => "String",
                            "bool" => "Bool",
                            "int" => "Int",
                            _ => throw new Exception($"Bad field type {fieldThatNeedsSaving.Name} {fieldThatNeedsSaving.Type} {fieldThatNeedsSaving.Container.Name} {fieldThatNeedsSaving.Container.Namespace}")
                        };

                        writer.WriteLine($"if (tag.ContainsKey(nameof({fieldThatNeedsSaving.Name}))) {fieldThatNeedsSaving.Name} = tag.Get{readName}(nameof({fieldThatNeedsSaving.Name}));");
                    }

                    writer.Indent--;
                    writer.WriteLine("}");
                }

                writer.Indent--;
                writer.WriteLine("}");
                writer.Indent--;
                writer.WriteLine("}");
            }

            sourceProductionContext.AddSource("NetSync.g.cs", text.ToString());
        });
    }
}