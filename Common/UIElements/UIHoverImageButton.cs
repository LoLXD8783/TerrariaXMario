using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace TerrariaXMario.Common.UIElements;

internal class UIHoverImageButton : UIImageButton
{
    private readonly LocalizedText hoverText;

    internal UIHoverImageButton(Asset<Texture2D> texture, LocalizedText hoverText) : base(texture)
    {
        this.hoverText = hoverText;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (IsMouseHovering) Main.hoverItemName = hoverText.Value;
    }
}
