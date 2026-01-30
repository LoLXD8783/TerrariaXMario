using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;

internal class PowItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    internal int powAdditive;
    internal int powMultiplier;

    public override void UpdateAccessory(Item item, Player player, bool hideVisual)
    {
        if (powAdditive != 0) player.GetModPlayerOrNull<CapEffectsPlayer>()?.statPowerAdditive += powAdditive;
        if (powMultiplier != 0) player.GetModPlayerOrNull<CapEffectsPlayer>()?.statPowerMultiplier += powMultiplier;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (powAdditive != 0)
        {
            int index = tooltips.FindIndex(e => e.Name == "Equipable");
            if (index != -1) tooltips.Insert(index + 1, new(Mod, "Power", $"{powAdditive} {Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.UI.BroInfoUI.Pow").ToLower()}"));
        }
    }
}
