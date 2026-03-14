using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria.UI;
using TerrariaXMario.Common.GearLoadout;

namespace TerrariaXMario.Common.GearAccessorySlots;

internal class ModifyEquipmentSlots : ILoadable
{
    private static bool Enabled(Player player) => player.TryGetModPlayer(out GearLoadoutPlayer modPlayer) && player.CurrentLoadoutIndex == modPlayer.gearLoadoutIndex;

    void ILoadable.Load(Mod mod)
    {
        // All of these modify armor and accessory slots in some way when gear loadout is selected

        // Prevents items from being quick-swapped into slots
        On_ItemSlot.ArmorSwap += DetourArmorSwap;

        // Removes visibility button from functional accessory slots
        MonoModHooks.Add(typeof(AccessorySlotLoader).GetMethod("DrawVisibility", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance), DetourDrawVisibility);
        MonoModHooks.Modify(typeof(AccessorySlotLoader).GetMethod("DrawVisibility", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance), ModifyDrawVisibility);

        // Gets draw position for Gear Loadout Button
        IL_Main.DrawLoadoutButtons += ModifyDrawLoadoutButtons;

        // Removes armor slots
        IL_Main.DrawInventory += ModifyDrawInventory;

        // Removes all other accessory slots
        MonoModHooks.Add(typeof(AccessorySlotLoader).GetMethod("Draw"), DetourDraw);
    }

    void ILoadable.Unload() { }

    private Item DetourArmorSwap(On_ItemSlot.orig_ArmorSwap orig, Item item, out bool success)
    {
        if (Enabled(Main.LocalPlayer) && !TerrariaXMarioItemSets.GearAccessoryItem[item.type])
        {
            success = false;
            return item;
        }

        return orig(item, out success);
    }

    private void ModifyDrawVisibility(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel target = c.DefineLabel();

        c.Next(MoveType.After, i => i.MatchCallOrCallvirt(typeof(Rectangle).GetMethod("Contains", [typeof(Point)])!), i => i.MatchBrfalse(out target!));

        c.EmitDelegate(() => Enabled(Main.LocalPlayer));
        c.EmitBrtrue(target);
    }

    delegate bool orig_DrawVisibility(AccessorySlotLoader self, ref bool visbility, int context, int xLoc, int yLoc, out int xLoc2, out int yLoc2, out Texture2D value4);
    private bool DetourDrawVisibility(orig_DrawVisibility orig, AccessorySlotLoader self, ref bool visbility, int context, int xLoc, int yLoc, out int xLoc2, out int yLoc2, out Texture2D value4)
    {
        bool skipCheck = orig(self, ref visbility, context, xLoc, yLoc, out xLoc2, out yLoc2, out value4);

        if (Enabled(Main.LocalPlayer))
        {
            value4 = Asset<Texture2D>.DefaultValue;
            skipCheck = false;
        }

        return skipCheck;
    }

    private void ModifyDrawLoadoutButtons(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel loopLabel = c.DefineLabel();
        ILLabel label = c.DefineLabel();

        c.Next(i => i.MatchStloc(7));

        c.EmitLdcI4(1);
        c.EmitAdd();

        c.Next(i => i.MatchStloc(13));
        c.Index -= 2;

        c.EmitLdloc((byte)11);
        c.EmitLdloc1();
        c.EmitDelegate((int i, Player player) => i == 3);
        c.EmitBrfalse(label);
        c.EmitLdloc((byte)12);
        c.EmitDelegate((Rectangle rectangle2) =>
        {
            GearLoadoutButton.ButtonPosition = rectangle2.Center.ToVector2();
        });
        c.EmitBr(loopLabel);
        c.MarkLabel(label);

        c.Next(MoveType.After, i => i.MatchBlt(out _));
        c.MarkLabel(loopLabel);
    }

    private void ModifyDrawInventory(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel label = c.DefineLabel();
        ILLabel continueLabel = c.DefineLabel();

        c.Next(MoveType.After, i => i.MatchLdsfld<Main>("EquipPage"), i => i.MatchBrtrue(out label!));

        c.EmitDelegate(() => !Enabled(Main.LocalPlayer));
        c.EmitBrtrue(continueLabel);
        c.EmitLdloc((byte)10);
        c.EmitDelegate(Main.LocalPlayer.CanDemonHeartAccessoryBeShown);
        c.EmitDelegate(Main.LocalPlayer.CanMasterModeAccessoryBeShown);
        c.EmitCall(typeof(Main).GetMethod("DrawLoadoutButtons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!);
        c.EmitDelegate(() => Main.screenWidth + 1);
        c.EmitDelegate(() => AccessorySlotLoader.DrawVerticalAlignment + 164 * Main.inventoryScale + 4f);
        c.EmitConvI4();
        c.EmitCall(typeof(Main).GetMethod("DrawDefenseCounter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!);
        c.EmitBr(label);
        c.MarkLabel(continueLabel);
    }

    delegate bool orig_Draw(AccessorySlotLoader self, int skip, bool modded, int slot, Color color);
    private bool DetourDraw(orig_Draw orig, AccessorySlotLoader self, int skip, bool modded, int slot, Color color)
    {
        if (!Enabled(Main.LocalPlayer)) return orig(self, skip, modded, slot, color);

        if (modded && self.Get(slot).Mod == TerrariaXMario.Instance) return orig(self, skip, modded, slot, color);

        if (AccessorySlotLoader.DefenseIconPosition == Vector2.Zero) return orig(self, skip, modded, slot, color);

        return false;
    }
}