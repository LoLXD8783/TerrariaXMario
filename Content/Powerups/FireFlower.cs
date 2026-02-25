using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.Projectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class FireFlower : Powerup
{
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];

    internal virtual int ProjectileType => ModContent.ProjectileType<FireFlowerFireball>();
    internal virtual float ProjectileGravity => 0.4f;

    private int upgrade;
    private int ingredientTypeForUpgrade3;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.UseSound = new($"{TerrariaXMario.Sounds}/PowerupEffects/{Name}Shoot") { Volume = 0.4f };
    }

    public override bool CanUseItem(Player player) => player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatSP >= (AltUse(player) ? 15 : 5);
    internal override void Use(Player player)
    {
        Main.NewText(upgrade);
        Vector2 velocity = TerrariaXMario.GetInitialProjectileVelocity(player, ProjectileGravity);
        int damage = player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatPower ?? 1;
        float knockback = damage * 0.05f;

        Projectile.NewProjectile(Item.GetSource_FromThis(), player.MountedCenter, velocity, ProjectileType, damage, knockback, player.whoAmI);

        for (int i = -1; i < 2; i++)
        {
            if (i == 0) continue;

            Projectile.NewProjectile(Item.GetSource_FromThis("Mini Fireball"), player.MountedCenter, Vector2.Transform(velocity, Matrix.CreateRotationZ(MathHelper.PiOver4 * 0.25f * i)), ProjectileType, damage, knockback, player.whoAmI);
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = Recipe.Create(Type)
            .AddTile(TileID.CookingPots)
            .AddIngredient(Type);

        switch (upgrade)
        {
            case 0:
                recipe.AddIngredient(ItemID.MeteoriteBar, 8);
                break;
            case 1:
                recipe.AddIngredient(ItemID.HellstoneBar, 12);
                break;
            case 2:
                recipe.AddIngredient(ItemID.CursedFlame, 16);
                Recipe.Create(Type)
                    .AddTile(TileID.CookingPots)
                    .AddIngredient(Type)
                    .AddIngredient(ItemID.Ichor, 16)
                    .Register();
                break;
            case 3:
                recipe.AddIngredient(ItemID.SunStone);
                break;
            case 4:
                recipe.AddIngredient(ItemID.FragmentSolar, 24);
                break;
            default:
                return;
        }

        recipe.Register();
    }

    public override void OnCreated(ItemCreationContext context)
    {
        if (context is not RecipeItemCreationContext recipeContext) return;

        upgrade = (int)MathHelper.Min(5, upgrade + 1);
    }
}