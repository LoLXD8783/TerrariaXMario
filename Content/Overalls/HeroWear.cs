using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class HeroWear : OverallsItem
{
    internal override int Defense => 38;
    internal override int PowAdditive => 54;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 80);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
