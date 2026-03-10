using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace TerrariaXMario.Utilities.Assets;

internal struct ImageData(string path)
{
    internal string Path = path;

    internal readonly Asset<Texture2D> Get => ModContent.Request<Texture2D>(Path);
    internal readonly Texture2D GetValue => Get.Value;
}