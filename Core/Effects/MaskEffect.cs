using Microsoft.Xna.Framework.Graphics;

namespace TerrariaXMario.Core.Effects;

internal abstract class MaskEffect : ModSystem
{
    internal virtual bool Enabled => false;
    internal virtual Vector2 Size => Vector2.Zero;

    internal Vector2 ScreenPosition;
    internal float Scale;

    private static GraphicsDevice GraphicsDevice => Main.graphics.GraphicsDevice;

    private RenderTargetBinding[] oldRenderTargets = [];
    private static RenderTarget2D? maskRenderTarget;

    internal virtual void Init() { }
    internal virtual void Update() { }

    public override void Load()
    {
        Main.QueueMainThreadAction(() =>
        {
            maskRenderTarget = new(GraphicsDevice, Main.screenWidth, Main.screenHeight);
        });

        Main.OnPreDraw += Main_OnPreDraw;
    }

    private static readonly BlendState AlphaCutoutBlend = new()
    {
        ColorSourceBlend = Blend.Zero,
        ColorDestinationBlend = Blend.One,
        ColorBlendFunction = BlendFunction.Add,

        AlphaSourceBlend = Blend.One,
        AlphaDestinationBlend = Blend.Zero,
        AlphaBlendFunction = BlendFunction.Add
    };

    private void Main_OnPreDraw(GameTime obj)
    {
        oldRenderTargets = GraphicsDevice.GetRenderTargets();
        GraphicsDevice.SetRenderTarget(maskRenderTarget);
        GraphicsDevice.Clear(Color.Black);

        Main.spriteBatch.Begin(SpriteSortMode.Immediate, AlphaCutoutBlend, SamplerState.PointClamp, null, null);
        Main.spriteBatch.Draw(ModContent.Request<Texture2D>(GetType().FullName!.Replace(".", "/")).Value, ScreenPosition, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        Main.spriteBatch.End();

        GraphicsDevice.SetRenderTargets(oldRenderTargets);
        oldRenderTargets = [];
    }

    public override void PostDrawTiles()
    {
        if (!Enabled)
        {
            Init();
            return;
        }

        Update();

        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        Main.spriteBatch.Draw(maskRenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
        Main.spriteBatch.End();
    }
}