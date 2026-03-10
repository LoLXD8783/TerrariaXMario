using Terraria.Localization;
using Terraria.UI;
using TerrariaXMario.Common.UIElements;
using TerrariaXMario.Utilities.Assets;

namespace TerrariaXMario.Common.GearLoadout;

[Autoload(Side = ModSide.Client)]
internal class GearLoadoutButtonSystem : ModSystem
{
    internal static Vector2 GearLoadoutButtonPosition = -Main.ScreenSize.ToVector2();

    private UserInterface? GearLoadoutButtonUserInterface;
    private GearLoadoutButtonState? GearLoadoutButtonState;

    public override void Load()
    {
        GearLoadoutButtonState = new();
        GearLoadoutButtonUserInterface = new();
        GearLoadoutButtonUserInterface.SetState(GearLoadoutButtonState);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        GearLoadoutButtonUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer($"{nameof(TerrariaXMario)}: Gear Loadout Button", () =>
        {
            if (Main.playerInventory) GearLoadoutButtonUserInterface?.Draw(Main.spriteBatch, new());
            return true;
        }, InterfaceScaleType.UI));
    }
}

internal class GearLoadoutButtonState : UIState
{
    private UIHoverImageButton? GearLoadoutButton { get; set; }

    public override void OnInitialize()
    {
        GearLoadoutButton = this.AddElement(new UIHoverImageButton(Assets.GearLoadoutButton.Get, Language.Get($"UI.GearLoadoutButton")).With(e =>
        {
            e.Left = StyleDimension.FromPixels(GearLoadoutButtonSystem.GearLoadoutButtonPosition.X - 20);
            e.Top = StyleDimension.FromPixels(GearLoadoutButtonSystem.GearLoadoutButtonPosition.Y - 16);
            e.Width = StyleDimension.FromPixels(32);
            e.Height = StyleDimension.FromPixels(32);
            e.SetVisibility(1, 1);
            e.SetHoverImage(Assets.GearLoadoutButtonHover.Get);
            e.OnLeftClick += GearLoadoutButtonClick;
        }));
    }

    private void GearLoadoutButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        Player player = Main.LocalPlayer;
        player.TrySwitchingLoadout(player.GearLoadoutPlayer.gearLoadoutIndex);
    }

    public override void Update(GameTime gameTime)
    {
        bool enabled = Main.LocalPlayer.GearLoadoutPlayer.Enabled;

        GearLoadoutButton?.SetImage((enabled ? Assets.GearLoadoutButtonActive : Assets.GearLoadoutButton).Get);
        GearLoadoutButton?.SetHoverImage((enabled ? Assets.GearLoadoutButtonActiveHover : Assets.GearLoadoutButtonHover).Get);

        GearLoadoutButton?.Left = StyleDimension.FromPixels(GearLoadoutButtonSystem.GearLoadoutButtonPosition.X - 18);
        GearLoadoutButton?.Top = StyleDimension.FromPixels(GearLoadoutButtonSystem.GearLoadoutButtonPosition.Y - 18);

        if (GearLoadoutButton?.IsMouseHovering ?? false) Main.LocalPlayer.mouseInterface = true;
    }
}