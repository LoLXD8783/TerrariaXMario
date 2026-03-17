using MonoMod.Cil;

namespace TerrariaXMario.Common.WallKick;

[CanBeReadBySourceGenerators]
internal class WallKickPlayer : ModPlayer
{
    public override void Load()
    {
        // Sets player speed when jumping off a wall
        IL_Player.JumpMovement += ModifyJumpMovement;
    }

    private void ModifyJumpMovement(ILContext il)
    {
        ILCursor c = new(il);

        c.Next(i => i.MatchLdfld<Player>("slideDir"));
        c.Index--;

        c.EmitPop();
        c.EmitLdarg(0);
        c.EmitDelegate((Player player) => player.CapPlayer.Enabled ? 5 : 3);
    }
}
