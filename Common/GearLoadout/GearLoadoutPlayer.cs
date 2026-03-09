using MonoMod.Cil;
using Terraria.ModLoader.Default;

namespace TerrariaXMario.Common.GearLoadout;

[CanBeReadBySourceGenerators]
internal partial class GearLoadoutPlayer : ModPlayer
{
    internal int gearLoadoutIndex;

    internal bool IsGearLoadoutIndex(int loadoutIndex) => loadoutIndex == gearLoadoutIndex;
    internal bool IsGearLoadoutIndex() => Player.CurrentLoadoutIndex == gearLoadoutIndex;

    [NetSync(ShouldSave = true)] internal bool Enabled { get; set; }

    public override void Load()
    {
        // Allows switching to loadout index of Player.Loadouts.Length
        IL_Player.TrySwitchingLoadout += ModifyTrySwitchingLoadout;
        MonoModHooks.Modify(typeof(ModAccessorySlotPlayer).GetMethod("OnEquipmentLoadoutSwitched"), ModifyOnEquipmentLoadoutSwitched);
        IL_Player.Deserialize_PlayerFileData_Player_BinaryReader_int_refBoolean += ModifyDeserialize;
    }

    public override void Initialize()
    {
        gearLoadoutIndex = Player.Loadouts.Length;
    }

    private void ModifyTrySwitchingLoadout(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel label1 = c.DefineLabel();
        ILLabel label2 = c.DefineLabel();

        c.Next(MoveType.After, i => i.MatchConvI4());

        c.EmitLdcI4(1);
        c.EmitAdd();

        c.Next(MoveType.After, i => i.MatchStloc1());

        c.EmitLdarg0();
        c.EmitDelegate((Player player) => player.GearLoadoutPlayer.IsGearLoadoutIndex());
        c.EmitBrtrue(label1);

        c.Next(MoveType.After, i => i.MatchCallvirt<EquipmentLoadout>("Swap"));
        c.MarkLabel(label1);

        c.EmitLdarg0();
        c.EmitLdarg1();
        c.EmitDelegate((Player player, int loadoutIndex) => player.GearLoadoutPlayer.IsGearLoadoutIndex(loadoutIndex));
        c.EmitBrtrue(label2);

        c.Next(MoveType.After, i => i.MatchCallvirt<EquipmentLoadout>("Swap"));
        c.MarkLabel(label2);

        c.Next(i => i.MatchRet());

        c.EmitLdarg0();
        c.EmitDelegate((Player player) =>
        {
            player.GearLoadoutPlayer.Enabled = player.GearLoadoutPlayer.IsGearLoadoutIndex();
        });
    }

    private void ModifyOnEquipmentLoadoutSwitched(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel label1 = c.DefineLabel();
        ILLabel label2 = c.DefineLabel();

        c.EmitLdarg0();
        c.EmitLdarg1();
        c.EmitDelegate((ModAccessorySlotPlayer player, int loadoutIndex) => player.Player.GearLoadoutPlayer.IsGearLoadoutIndex(loadoutIndex));
        c.EmitBrtrue(label1);

        c.Next(i => i.MatchLdarg2());
        c.Index -= 2;
        c.MarkLabel(label1);

        c.EmitLdarg0();
        c.EmitLdarg2();
        c.EmitDelegate((ModAccessorySlotPlayer player, int loadoutIndex) => player.Player.GearLoadoutPlayer.IsGearLoadoutIndex(loadoutIndex));
        c.EmitBrtrue(label2);

        c.Next(i => i.MatchLdarg0(), i => i.MatchCall<ModPlayer>("get_Player"));
        c.MarkLabel(label2);
    }

    private void ModifyDeserialize(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel label = c.DefineLabel();

        c.Next(i => i.MatchStfld<Player>("CurrentLoadoutIndex"));
        c.Index -= 3;

        c.RemoveRange(2);
    }
}
