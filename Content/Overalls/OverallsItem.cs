using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.BroInfoUI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Overalls;

internal abstract class OverallsItem : ModItem
{
    internal static bool DoGearItemEffects(Player player) => player.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? false;

    internal virtual int Defense => 0;
    internal virtual int PowAdditive => 0;
    internal virtual int PowMultiplier => 0;

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 32;
        Item.accessory = true;
        Item.GetGlobalItemOrNull<PowItem>()?.powAdditive = PowAdditive;
        Item.GetGlobalItemOrNull<PowItem>()?.powMultiplier = PowMultiplier;

        Item.GetGlobalItemOrNull<GearSlotGlobalItem>()?.gearType = GearType.Overalls;
    }

    public override bool CanEquipAccessory(Player player, int slot, bool modded) => modded;

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.statDefense += Defense;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        int index = tooltips.FindIndex(e => e.Name == "Equipable");

        if (index != -1) tooltips.Insert(index + 1, new(Mod, "Defense", $"{Defense} {Lang.inter[10].Value.ToLower()}"));
    }
}
