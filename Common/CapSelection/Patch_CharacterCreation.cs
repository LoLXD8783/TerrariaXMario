using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapSelection;

internal class Patch_CharacterCreation : BasePatch
{
    internal override void Patch(Mod mod)
    {
        IL_UICharacterCreation.MakeInfoMenu += IL_UICharacterCreation_MakeInfoMenu;
    }

    private void IL_UICharacterCreation_MakeInfoMenu(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(i => i.MatchStloc1())) ThrowError("stloc1");
        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0))) ThrowError("ldcr4");

        c.EmitPop();
        c.EmitLdcR4(-100);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0.5f))) ThrowError("ldcr4");

        c.EmitPop();
        c.EmitLdcR4(0);

        if (!c.TryGotoNext(i => i.MatchStfld<UICharacterCreation>("_difficultyDescriptionText"))) ThrowError("stfld");
        c.Index++;
        c.EmitLdloc0();
        c.EmitLdloc1();
        c.EmitLdarg0();
        c.EmitLdfld(typeof(UICharacterCreation).GetField("_difficultyDescriptionText", BindingFlags.NonPublic | BindingFlags.Instance)!);
        c.EmitLdarg0();
        c.EmitLdfld(typeof(UICharacterCreation).GetField("_player", BindingFlags.NonPublic | BindingFlags.Instance)!);
        c.EmitDelegate((UIElement parent, UICharacterNameButton nameButton, UIText hoverText, Player player) =>
        {
            nameButton.Top = StyleDimension.FromPixels(2);
            CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();
            string texturePath = $"{nameof(TerrariaXMario)}/Content/Caps/";

            UIColoredImageButton mario = parent.AddElement(new UIColoredImageButton(ModContent.Request<Texture2D>($"{texturePath}Mario")).With(e =>
             {
                 e.SetVisibility(1, 1);
                 e.SetSelected(true);
                 e.HAlign = 1;
                 e.Left = StyleDimension.FromPixels(-50);
             }));

            UIColoredImageButton luigi = parent.AddElement(new UIColoredImageButton(ModContent.Request<Texture2D>($"{texturePath}Luigi")).With(e =>
            {
                e.SetVisibility(1, 1);
                e.SetSelected(false);
                e.HAlign = 1;
            }));

            mario.OnLeftMouseDown += (evt, listeningElement) =>
            {
                mario.SetSelected(true);
                luigi.SetSelected(false);
                modPlayer?.startingCap = "Mario";
            };

            mario.OnMouseOver += (evt, listeningElement) =>
            {
                hoverText.SetText(Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.UI.CapSelect.Mario"));
                modPlayer?.currentCap = "Mario";
            };

            mario.OnMouseOut += (evt, listeningElement) =>
            {
                hoverText.SetText(GetDifficultyDescription(player));
                modPlayer?.currentCap = null;
            };

            luigi.OnLeftMouseDown += (evt, listeningElement) =>
            {
                mario.SetSelected(false);
                luigi.SetSelected(true);
                modPlayer?.startingCap = "Luigi";
            };

            luigi.OnMouseOver += (evt, listeningElement) =>
            {
                hoverText.SetText(Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.UI.CapSelect.Luigi"));
                modPlayer?.currentCap = "Luigi";
            };

            luigi.OnMouseOut += (evt, listeningElement) =>
            {
                hoverText.SetText(GetDifficultyDescription(player));
                modPlayer?.currentCap = null;
            };
        });
    }

    private static LocalizedText GetDifficultyDescription(Player player)
    {
        LocalizedText text = Lang.menu[31];
        switch (player.difficulty)
        {
            case 0:
                text = Lang.menu[31];
                break;
            case 1:
                text = Lang.menu[30];
                break;
            case 2:
                text = Lang.menu[29];
                break;
            case 3:
                text = Language.GetText("UI.CreativeDescriptionPlayer");
                break;
        }

        return text;
    }
}
