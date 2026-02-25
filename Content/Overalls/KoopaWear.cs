using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class KoopaWear : OverallsItem
{
    internal override int Defense => 62;
    internal override int PowAdditive => 78;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 40);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}