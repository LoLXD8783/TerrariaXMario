using Terraria.Localization;
using Terraria.UI;
using TerrariaXMario.Common.UIElements;
using TerrariaXMario.Utilities.AssetData;

namespace TerrariaXMario.Common.GearLoadout;

[CanBeReadBySourceGenerators]
internal class GearLoadoutButton : BetterUIState
{
    internal static Vector2 ButtonPosition = -Main.ScreenSize.ToVector2();

    internal override bool Visible => Main.playerInventory;

    private static bool LoadoutEnabled => Player.GearLoadoutPlayer.Enabled;

    private UIHoverImageButton? Button { get; set; }

    public override void OnInitialize()
    {
        Button = this.AddElement(new UIHoverImageButton(Assets.GearLoadoutButton.Get, Language.Get($"UI.GearLoadoutButton")).With(e =>
        {
            e.Left = StyleDimension.FromPixels(ButtonPosition.X - 20);
            e.Top = StyleDimension.FromPixels(ButtonPosition.Y - 16);
            e.Width = StyleDimension.FromPixels(32);
            e.Height = StyleDimension.FromPixels(32);
            e.SetVisibility(1, 1);
            e.SetHoverImage(Assets.GearLoadoutButtonHover.Get);
            e.OnLeftClick += GearLoadoutButtonClick;
        }));
    }

    private void GearLoadoutButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        Player.TrySwitchingLoadout(Player.GearLoadoutPlayer.gearLoadoutIndex);
    }

    public override void Update(GameTime gameTime)
    {
        Button?.SetImage((LoadoutEnabled ? Assets.GearLoadoutButtonActive : Assets.GearLoadoutButton).Get);
        Button?.SetHoverImage((LoadoutEnabled ? Assets.GearLoadoutButtonActiveHover : Assets.GearLoadoutButtonHover).Get);

        Button?.Left = StyleDimension.FromPixels(ButtonPosition.X - 18);
        Button?.Top = StyleDimension.FromPixels(ButtonPosition.Y - 18);

        if (Button?.IsMouseHovering ?? false) Player.mouseInterface = true;
    }
}