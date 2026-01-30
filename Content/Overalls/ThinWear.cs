using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.BroInfoUI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Overalls;

internal class ThinWear : ModItem
{
    internal virtual int Defense => 2;
    internal virtual int PowAdditive => 2;
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
        if (!player.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true) return;

        player.statDefense += Defense;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        int index = tooltips.FindIndex(e => e.Name == "Equipable");

        if (index != -1) tooltips.Insert(index + 1, new(Mod, "Defense", $"{Defense} {Lang.inter[10].Value.ToLower()}"));
    }
}
