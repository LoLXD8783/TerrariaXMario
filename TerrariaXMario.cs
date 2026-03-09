using Terraria.ID;

namespace TerrariaXMario;

/// <summary>
/// Indicates that a class can be read and processed by <c>TerrariaXMario.SourceGenerators</c>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class CanBeReadBySourceGeneratorsAttribute : Attribute;

/// <summary>
/// Indicates that a field can be read and processed by <c>TerrariaXMario.SourceGenerators.NetSyncGenerator</c>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
internal class NetSyncAttribute : Attribute
{
    public bool ShouldSave { get; set; }
}

internal partial class TerrariaXMario : Mod
{
    internal static TerrariaXMario Instance => ModContent.GetInstance<TerrariaXMario>();
}

[ReinitializeDuringResizeArrays]
internal static class TerrariaXMarioItemSets
{
    public static bool[] GearAccessoryItem = ItemID.Sets.Factory.CreateNamedSet("GearAccessoryItem")
        .Description("If the item is a gear accessory item.")
        .RegisterBoolSet(false);
}