using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using TerrariaXMario.Common.UIElements;
using TerrariaXMario.Utilities.Assets;

namespace TerrariaXMario.Common.PMeter;

[CanBeReadBySourceGenerators]
internal class PMeter : BetterUIState
{
    internal override bool Visible => Player.CapPlayer.Enabled;
    internal override InterfaceScaleType InterfaceScaleType => InterfaceScaleType.Game;

    private readonly int count = 5;
    private readonly int flickerTime = 10;
    private int flickerTimer = 0;
    private int colorIndex = 0;
    private float opacity;

    private static Texture2D Texture => Assets.PMeter.GetValue;
    private Vector2 DrawPosition => PMeterContainer?.GetDimensions().Position() ?? Vector2.Zero;
    private Color DrawColor => Color.White * opacity;
    private static float FillAmount => Player.PMeterPlayer.Charge;

    private UIElement? PMeterContainer { get; set; }

    public override void OnInitialize()
    {
        PMeterContainer = this.AddElement(new UIElement().With(e =>
        {
            e.HAlign = 0.5f;
            e.Height = StyleDimension.FromPixels(16);
        }));
    }

    public override void Update(GameTime gameTime)
    {
        if (PMeterContainer == null) return;

        PMeterContainer.Width = StyleDimension.FromPixels(26 + 6 * (count - 1));
        PMeterContainer.Top = StyleDimension.FromPixels(Player.Bottom.Y + 16 - Main.screenPosition.Y);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (PMeterContainer == null) return;

        if (FillAmount == 1)
        {
            if (flickerTimer > 0) flickerTimer--;
            else
            {
                flickerTimer = flickerTime;
                colorIndex = colorIndex == 1 ? 2 : 1;
            }
        }
        else
        {
            flickerTimer = 0;
            colorIndex = 0;
            opacity = float.Lerp(opacity, FillAmount >= 0.5f ? 1 : 0, 0.1f);
        }

        // Static elements
        spriteBatch.Draw(Texture, DrawPosition + new Vector2(0, 4), new(0, 12 * colorIndex, 10, 10), DrawColor);
        spriteBatch.Draw(Texture, DrawPosition + new Vector2(12 + 6 * (count - 1), 0), new(16 * colorIndex, 36, 14, 16), DrawColor);

        // Bar
        for (int i = 0; i < count; i++)
        {
            float size = 1 / (float)count;

            if (i != count - 1) spriteBatch.Draw(Texture, DrawPosition + new Vector2(8 + 6 * i, 4), new(12, 12 * colorIndex, 8, 10), DrawColor);
            if (colorIndex == 0) spriteBatch.Draw(Texture, DrawPosition + new Vector2(2 + 6 * i, 4), new(22, 0, (int)((FillAmount - size * i) / size * 8), 10), DrawColor);
        }
    }
}