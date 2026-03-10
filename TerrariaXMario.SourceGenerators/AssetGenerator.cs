using Microsoft.CodeAnalysis;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TerrariaXMario.SourceGenerators;
/// <summary>
/// Generates a file containing fields for all image and audio files
/// </summary>

[Generator(LanguageNames.CSharp)]
public sealed class AssetGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var collectedAssets = context.AdditionalTextsProvider.Where(file =>
                file.Path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                file.Path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            .Select((file, cancellationToken) => file.Path).Collect();

        context.RegisterSourceOutput(collectedAssets, (sourceProductionContext, assets) =>
        {
            StringBuilder text = new(2048);
            using IndentedTextWriter writer = new(new StringWriter(text));

            writer.WriteLine("namespace TerrariaXMario.Utilities.Assets;");
            writer.WriteLine("internal static class Assets");
            writer.WriteLine("{");
            writer.Indent++;

            string[] caps = ["Mario"];
            bool isCapAsset(string path) => caps.Any(j => path.Contains($"Content\\{j}"));

            writer.WriteLine("internal static Dictionary<string, CapAudioData> CapAudio = new()");
            writer.WriteLine("{");
            writer.Indent++;

            foreach (string cap in caps)
            {
                writer.WriteLine("{ \"" + $"{cap}\",");
                writer.Indent++;
                writer.WriteLine("new() {");
                writer.Indent++;

                string currentAsset = "";

                foreach (string asset in assets.Where(i => i.Contains($"Content\\{cap}")).OrderBy(i => i, StringComparer.OrdinalIgnoreCase))
                {
                    string type = asset.Split('.').Last() switch
                    {
                        "png" => "Image",
                        "wav" => "Audio",
                        _ => "None"
                    };

                    if (type != "Audio") continue;

                    List<string> directories = [.. asset.Split('.').First().Split('\\')];
                    directories.RemoveRange(0, 8);

                    string name = Regex.Replace(directories.Last(), @"\d+$", "").Replace(cap, "");
                    string path = string.Join("/", directories).Split('.').First();

                    if (currentAsset != name)
                    {
                        if (currentAsset != "")
                        {
                            writer.Indent--;
                            writer.WriteLine("),");
                        }

                        writer.WriteLine($"{name} = new(");
                        writer.Indent++;

                    }

                    writer.WriteLine($"{(currentAsset != name ? "" : ",")}\"{path}\"");
                    currentAsset = name;
                }

                writer.Indent--;
                writer.WriteLine(")");
                writer.Indent--;
                writer.WriteLine("}");
                writer.Indent--;
                writer.WriteLine("},");
            }
            writer.Indent--;
            writer.WriteLine("};");

            foreach (string asset in assets.Where(i => !isCapAsset(i)))
            {
                string type = asset.Split('.').Last() switch
                {
                    "png" => "Image",
                    "wav" => "Audio",
                    _ => "None"
                };

                if (type == "None") continue;

                List<string> directories = [.. asset.Split('.').First().Split('\\')];
                directories.RemoveRange(0, 8);

                string path = string.Join("/", directories).Split('.').First();

                writer.WriteLine($"internal static {type}Data {directories.Last()} = new(\"{path}\");");
            }

            writer.Indent--;
            writer.WriteLine("}");

            sourceProductionContext.AddSource("Assets.g.cs", text.ToString());
        });
    }
}