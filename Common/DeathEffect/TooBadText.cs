using MonoMod.Cil;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.Localization;
using TerrariaXMario.Utilities.AssetData;

namespace TerrariaXMario.Common.DeathEffect;

internal class TooBadText : ModSystem
{
    public override void Load()
    {
        // Replaces the default "You were slain" text with "TOO BAD!" using a custom font
        IL_Main.DrawInterface_35_YouDied += ModifyDrawInterfaceYouDied;
    }

    private void ModifyDrawInterfaceYouDied(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel label = c.DefineLabel();

        c.Next(i => i.MatchStloc1());
        c.EmitPop();
        c.EmitDelegate(() => Main.LocalPlayer.CapPlayer.Enabled ? Language.GetValue("UI.DeathText") : Lang.inter[38].Value);

        for (int j = 0; j < 2; j++)
        {
            c.Next(MoveType.After, i => i.MatchCallvirt<Asset<DynamicSpriteFont>>("get_Value"));
            c.EmitPop();
            c.EmitDelegate(() => Main.LocalPlayer.CapPlayer.Enabled ? Assets.MarioFont.GetValue : FontAssets.DeathText.Value);
        }

        c.Next(MoveType.After, i => i.MatchLdloc0(), i => i.MatchAdd());
        c.EmitDelegate(() => Main.LocalPlayer.CapPlayer.Enabled);
        c.EmitBrfalse(label);
        c.EmitLdcR4(20);
        c.EmitSub();
        c.MarkLabel(label);
    }
}
