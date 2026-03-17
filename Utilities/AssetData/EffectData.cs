using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace TerrariaXMario.Utilities.AssetData;

internal struct EffectData(string path)
{
    internal string Path = path;

    internal readonly Asset<Effect> Get => ModContent.Request<Effect>(Path, AssetRequestMode.ImmediateLoad);
    internal readonly Effect GetValue => Get.Value;

    internal readonly void Apply(int technique = 0, int pass = 0)
    {
        GetValue.Techniques[technique].Passes[pass].Apply();
    }
}