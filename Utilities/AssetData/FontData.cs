using ReLogic.Content;
using ReLogic.Graphics;

namespace TerrariaXMario.Utilities.AssetData;

internal struct FontData(string path)
{
    internal string Path = path;

    internal readonly Asset<DynamicSpriteFont> Get => ModContent.Request<DynamicSpriteFont>(Path);
    internal readonly DynamicSpriteFont GetValue => Get.Value;
}