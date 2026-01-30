using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.SPBar;

internal class SPBar : UIState
{
    private UIElement? Container { get; set; }

    public override void OnInitialize()
    {
        string path = GetType().Namespace!.Replace(".", "/");

        Container = this.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixels(40);
            e.HAlign = 1;
            e.Left = StyleDimension.FromPixels(-48);
            e.Top = StyleDimension.FromPixels(84);
        }));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer == null || !modPlayer.CanDoCapEffects) return;

        if (Container?.IsMouseHovering ?? false) Main.hoverItemName = $"{modPlayer.StatSP}/{modPlayer.MaxSP}";

        string path = GetType().Namespace!.Replace(".", "/");
        bool modern = TerrariaXMario.ResourceBarStyle.Contains("Bars");
        bool fancy = TerrariaXMario.ResourceBarStyle.Contains("Fancy");
        Vector2 position = Container!.GetDimensions().Position();

        if (fancy && modPlayer.MaxSP <= 20)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SPSingle").Value, position, Color.White);
        }

        for (int i = 0; i < (float)modPlayer.MaxSP / 20; i++)
        {
            bool bottom = i == (float)modPlayer.MaxSP / 20 - 1;

            if (modern) spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SPMiddleModern").Value, position + new Vector2(6, 38 + i * 12), null, Color.White);

            if (fancy || (modern && bottom && i != 0)) spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SP{(i == 0 ? "Top" : bottom ? "Bottom" : "Middle")}{(modern ? "Modern" : "")}").Value, position + new Vector2(modern && bottom ? 6 : 0, modern && bottom ? 38 + (i + 1) * 12 : i == 0 ? 0 : 34 + 32 * (i - 1)), null, Color.White);

            int segmentStart = i * 20;
            int segmentSize = Math.Min(modPlayer.MaxSP, segmentStart + 20) - segmentStart;

            float scale = 0;

            if (segmentSize > 0)
            {
                int valueInSegment = Math.Clamp(modPlayer.StatSP - segmentStart, 0, segmentSize);
                scale = (float)valueInSegment / segmentSize;
            }

            if (!fancy && !modern) scale = scale * 0.5f + 0.5f;

            spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SPFill{(modern ? "Modern" : "")}").Value, modern ? position + new Vector2(28, 53 + i * 12) : position + new Vector2(20, 19) + new Vector2(0, i == 0 ? 0 : 32 + 32 * (i - 1)), modern ? new Rectangle(0, 0, 12, (int)(12 * scale)) : null, Color.White * (fancy || modern ? 1 : scale), 0, new Vector2(16, 15), modern ? 1 : scale, SpriteEffects.None, 0);
        }

        if (modern)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SPTopModern").Value, position, null, Color.White);
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SPBottomModern").Value, position + new Vector2(6, 38 + (float)Math.Ceiling((double)modPlayer.MaxSP / 20) * 12), null, Color.White);
        }
    }

    internal void ChangeResourceSet(string name)
    {
        if (Container == null) return;

        bool bars = name.Contains("Bars");

        Container.Left = StyleDimension.FromPixels(bars ? -3 : -48);
        Container.Top = StyleDimension.FromPixels(bars ? 30 + (int.TryParse(name.Split(" ").Last(), result: out int variant) ? variant switch { 1 => 0, 2 => 4, 3 => 2, _ => 0 } : 0) : 84);
        Container.Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer == null || Container == null) return;

        ChangeResourceSet(TerrariaXMario.ResourceBarStyle);

        bool bars = TerrariaXMario.ResourceBarStyle.Contains("Bars");
        if (modPlayer.MaxSP <= 20) Container.Height = StyleDimension.FromPixels(bars ? 56 : 38);
        else Container.Height = StyleDimension.FromPixels(bars ? modPlayer.MaxSP / 20 * 12 + 44 : (70 + (modPlayer.MaxSP - 40) / 20 * 32));
    }
}
