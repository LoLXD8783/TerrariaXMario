using Terraria.Localization;
using TerrariaXMario.Common.GearLoadout;
using TerrariaXMario.Content.Cap;
using TerrariaXMario.Utilities.Assets;

namespace TerrariaXMario.Common.GearAccessorySlots;

internal class CanEquipItemWhenGearAccessorySlotsEnabled : GlobalItem
{
    // Prevents vanilla items from being equippable when gear loadout is enabled
    public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
    {
        if (!TerrariaXMarioItemSets.GearAccessoryItem[item.type] && player.GearLoadoutPlayer.Enabled) return false;
        return base.CanEquipAccessory(item, player, slot, modded);
    }
}

internal abstract class GearAccessorySlot<T> : ModAccessorySlot where T : ModItem
{
    internal static string TypeName => typeof(T).Name;
    internal static Vector2 GetCustomLocation(int slotOffsetX = 0, int slotOffsetY = 0) => new(Main.screenWidth - 92 + 47 * slotOffsetX, AccessorySlotLoader.DrawVerticalAlignment + slotOffsetY * 56 * Main.inventoryScale + (slotOffsetY > 2 ? 4 : 0));

    public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) => checkItem.ModItem is T && context == AccessorySlotType.FunctionalSlot;

    public override bool DrawDyeSlot => typeof(CapItem) is T;

    public override bool DrawVanitySlot => false;

    public override bool IsEnabled() => Player.TryGetModPlayer(out GearLoadoutPlayer player) && player.Enabled;

    public override bool IsHidden() => Main.EquipPage != 0;

    public override bool HasEquipmentLoadoutSupport => false;

    public override bool IsVisibleWhenNotEnabled() => false;

    public override void OnMouseHover(AccessorySlotType context)
    {
        if (context == AccessorySlotType.DyeSlot) base.OnMouseHover(context);
        else Main.hoverItemName = Language.GetValue($"UI.GearAccessorySlots.{TypeName}");
    }
}

internal class CapSlot : GearAccessorySlot<CapItem>
{
    public override Vector2? CustomLocation => GetCustomLocation();
    public override string FunctionalTexture => Assets.CapItem.Path;
}

internal class OverallsSlot : GearAccessorySlot<CapItem>
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 1);
    public override string FunctionalTexture => Assets.OverallsItem.Path;
}

internal class GlovesSlot : GearAccessorySlot<CapItem>
{
    public override Vector2? CustomLocation => GetCustomLocation(-1, 1);
    public override string FunctionalTexture => Assets.GlovesItem.Path;
}

internal class SocksSlot : GearAccessorySlot<CapItem>
{
    public override Vector2? CustomLocation => GetCustomLocation(-1, 2);
    public override string FunctionalTexture => Assets.SocksItem.Path;
}

internal class BootsSlot : GearAccessorySlot<CapItem>
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 2);
    public override string FunctionalTexture => Assets.BootsItem.Path;
}

internal class AccessorySlot : GearAccessorySlot<CapItem>
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 3);
    public override string FunctionalTexture => Assets.AccessoryItem.Path;
}