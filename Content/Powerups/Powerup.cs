using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal struct PowerupEquipVariation
{
    internal string name;
    internal EquipType[] equipTypes;

    internal PowerupEquipVariation(string name, params EquipType[] equipTypes)
    {
        this.name = name;
        this.equipTypes = equipTypes;
    }

    internal static readonly PowerupEquipVariation Default = new("", EquipType.Head, EquipType.Body, EquipType.Legs);
    internal static readonly PowerupEquipVariation GroundPound = new("GroundPound", EquipType.Legs);
    internal static readonly PowerupEquipVariation Flying = new("Flying", EquipType.Head, EquipType.Body, EquipType.Legs);
}

internal delegate void PowerupWorldMovement(Powerup powerup);

internal static class PowerupWorldMovementType
{
    internal static readonly PowerupWorldMovement None = (powerup) => { };

    internal static readonly PowerupWorldMovement Bounce = (powerup) =>
    {
        if (powerup.Item.oldVelocity.X == 0) powerup.direction = 1 * -Math.Sign(powerup.direction);
        powerup.Item.velocity.X = powerup.direction * 2f;
        if (powerup.Item.velocity.Y == 0) powerup.Item.velocity.Y = -5;
    };
}

internal abstract class Powerup : ModItem
{
    internal static bool IsType<T>(Powerup? powerup) where T : Powerup => powerup != null && powerup.Type == ModContent.ItemType<T>();

    internal virtual PowerupWorldMovement WorldMovement => PowerupWorldMovementType.None;

    /// <summary>
    /// Permanent Powerups provide all bonuses when held and only start their timer upon first use. Setting this to true will cause this Powerup to be consumed upon first use, providing all bonuses in the process. Defaults to false.
    /// </summary>
    internal virtual bool IsTemporary => false;

    internal virtual int FrameCount => 1;
    internal virtual bool CanShowTail => false;
    internal virtual bool CanShowCape => false;

    internal virtual string PowerUpSound => $"{TerrariaXMario.Sounds}/PowerupEffects/PowerUp";

    /// <summary>
    /// A list of the caps that can consume this Powerup.
    /// </summary>
    internal virtual string[] Caps => [];

    /// <summary>
    /// A list of texture variations this Powerup should use, along with their equip types. Defaults to a normal state with Head, Body, and Legs, and a ground pound state with Legs.
    /// </summary>
    internal virtual PowerupEquipVariation[] Variations => [PowerupEquipVariation.Default, PowerupEquipVariation.GroundPound];

    internal virtual void OnJumpHeldDown(Player player) { }
    internal virtual void Use(Player player) { }
    internal virtual void UpdateAmbientEffects(Player player) { }

    internal int direction;
    internal bool canDraw = true;
    internal bool justSpawned = true;

    internal static bool AltUse(Player player) => player.altFunctionUse == 2;

    private void LoadEquipTextures(string cap, string variation = "")
    {
        foreach (EquipType equipType in Variations.First(e => e.name == variation).equipTypes)
        {
            EquipLoader.AddEquipTexture(Mod, $"{Texture}{cap}{variation}_{equipType}", equipType, name: $"{Name}{cap}{variation}");
        }
    }

    private void SetupEquipTextures(string cap, string variation = "", bool head = true, bool body = true, bool legs = true)
    {
        if (head)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, $"{Name.Replace("Projectile", "")}{cap}{variation}", EquipType.Head);
            if (equipSlotHead != -1) ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }
        if (body)
        {
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, $"{Name.Replace("Projectile", "")}{cap}{variation}", EquipType.Body);
            if (equipSlotBody != -1)
            {
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            }
        }
        if (legs)
        {
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, $"{Name.Replace("Projectile", "")}{cap}{variation}", EquipType.Legs);
            if (equipSlotLegs != -1) ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server) return;

        for (int i = 0; i < Caps.Length; i++)
        {
            string cap = Caps[i];

            for (int j = 0; j < Variations.Length; j++)
            {
                LoadEquipTextures(cap, Variations[j].name);
            }
        }
    }

    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(1, FrameCount) { NotActuallyAnimating = true });
        Item.ResearchUnlockCount = 30;

        if (Main.netMode == NetmodeID.Server) return;

        for (int i = 0; i < Caps.Length; i++)
        {
            string cap = Caps[i];

            for (int j = 0; j < Variations.Length; j++)
            {
                SetupEquipTextures(cap, Variations[j].name);
            }
        }
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.maxStack = 9999;
        Item.consumable = IsTemporary;

        Item.useStyle = ItemUseStyleID.Swing;
        Item.noUseGraphic = true;
        Item.autoReuse = true;
        Item.useTurn = true;
        Item.useTime = 17;
        Item.useAnimation = 17;
    }

    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        gravity *= 2;

        if (justSpawned)
        {
            direction = Math.Sign(Item.velocity.X);
            if (direction == 0) direction = Main.rand.NextFromList(-1, 1);
            justSpawned = false;
        }

        Item.oldVelocity = Item.velocity;
        WorldMovement(this);
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        rotation = 0;

        if (Item.noGrabDelay > 1)
        {
            if (Main.GameUpdateCount % 5 == 0) canDraw = !canDraw;
        }
        else canDraw = true;

        if (!canDraw) return false;
        return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
    }

    public override void GrabRange(Player player, ref int grabRange)
    {
        grabRange = 0;
    }

    public override void HoldItem(Player player)
    {
        if (!IsTemporary) player.GetModPlayerOrNull<CapEffectsPlayer>()?.currentPowerup = new Item(Type).ModItem as Powerup;
    }

    public override bool? UseItem(Player player)
    {
        Use(player);
        return true;
    }
}