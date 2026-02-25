using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using TerrariaXMario.Common.BroInfoUI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.Emotes;
using TerrariaXMario.Content.Overalls;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Vendors;

[AutoloadHead]
internal class Toad : ModNPC
{
    internal int shop;

    private static int ShimmerHeadIndex;
    private static Profiles.StackedNPCProfile? NPCProfile;

    private readonly string[] gearTypes = [.. Enum.GetNames(typeof(GearType)).Where(e => e != "None" && e != "Cap")];

    public override void Load()
    {
        //ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 25;

        NPCID.Sets.ExtraFramesCount[Type] = 9;
        NPCID.Sets.AttackFrameCount[Type] = 4;
        NPCID.Sets.AttackAverageChance[Type] = 0;
        NPCID.Sets.HatOffsetY[Type] = 4;
        //NPCID.Sets.ShimmerTownTransform[Type] = true;

        NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<ToadEmote>();

        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
        {
            Velocity = 1f,
            Direction = -1
        };

        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        NPC.Happiness
            .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
            .SetBiomeAffection<DesertBiome>(AffectionLevel.Like)
            .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Princess, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Truffle, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Clothier, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Hate)
        ;

        NPCProfile = new Profiles.StackedNPCProfile(
            new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture))
        //new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
        );

        ContentSamples.NpcBestiaryRarityStars[Type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 10;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = new($"{TerrariaXMario.Sounds}/Toad/ToadKill2") { Volume = 0.4f };
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;

        AnimationType = NPCID.Guide;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement($"Mods.{nameof(TerrariaXMario)}.NPCs.Toad.Bestiary"),
        ]);
    }

    public override bool CanTownNPCSpawn(int numTownNPCs) => true;

    public override ITownNPCProfile TownNPCProfile()
    {
        return NPCProfile;
    }

    public override List<string> SetNPCNameList()
    {
        return [
                "Toadbert",
                "Toadley",
                "Russ T.",
                "Mint T.",
                "Taste T.",
                "Tayce T.",
                "Juniper",
                "Keegan"
            ];
    }

    public override string GetChat()
    {
        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();
        bool showShop = modPlayer?.CanDoCapEffects ?? false;

        WeightedRandom<string> chat = new();
        string key = $"Mods.{nameof(TerrariaXMario)}.NPCs.Toad.Dialogue.{(showShop ? "" : "Alt.")}";

        for (int i = 0; i < 4; i++)
        {
            chat.Add(Language.GetText($"{key}{i}").Format(modPlayer?.currentCap ?? ""));
        }

        string chosenChat = chat;

        return chosenChat;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        if (Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>()?.CanDoCapEffects ?? false)
        {
            button = Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.NPCs.Toad.Shop.{gearTypes[shop]}");
            //button2 = Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.UI.CycleShop");
        }
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {

        if (firstButton) shop = gearTypes[this.shop];
        else this.shop = this.shop == gearTypes.Length - 1 ? 0 : this.shop + 1;
    }

    public override void AddShops()
    {
        foreach (string item in gearTypes)
        {
            var npcShop = new NPCShop(Type, item);

            switch (item)
            {
                case "Overalls":
                    npcShop.Add<ThinWear>();
                    npcShop.Add<PicnicWear>();
                    npcShop.Add<LeisureWear>();
                    npcShop.Add<FighterWear>(Condition.DownedKingSlime);
                    npcShop.Add<HeartWear>(Condition.DownedEyeOfCthulhu);
                    npcShop.Add<BrawnyWear>(Condition.DownedSkeletron);
                    npcShop.Add<GrownUpWear>(Condition.Hardmode);
                    npcShop.Add<KoopaWear>();
                    npcShop.Add<HeroWear>(Condition.DownedMechBossAny);
                    npcShop.Add<BalmWear>(Condition.DownedPirates);
                    npcShop.Add<MuscleWear>(Condition.DownedMechBossAll);
                    npcShop.Add<MasterWear>(Condition.DownedPlantera);
                    npcShop.Add<KingWear>(Condition.DownedGolem);
                    npcShop.Add<StarWear>(Condition.DownedMartians);
                    npcShop.Add<DStarWear>(Condition.DownedCultist);
                    npcShop.Add<AOKWear>(Condition.DownedMoonLord);
                    break;
                default:
                    break;
            }

            npcShop.Register();
        }
    }

    public override void ModifyActiveShop(string shopName, Item[] items)
    {
        foreach (Item item in items)
        {
            if (item == null || item.type == ItemID.None) continue;

            ToadShopConditionsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<ToadShopConditionsPlayer>();
            if (modPlayer == null) break;

            if (item.type == ModContent.ItemType<PicnicWear>() && !modPlayer.wentThroughNight) item.TurnToAir();
            if (item.type == ModContent.ItemType<LeisureWear>() && !modPlayer.beenToEvilBiome) item.TurnToAir();
            if (item.type == ModContent.ItemType<KoopaWear>() && !modPlayer.hasKilledTortoise) item.TurnToAir();
        }
    }
}

internal class ToadShopConditionsPlayer : ModPlayer
{
    internal bool wentThroughNight;
    internal bool beenToEvilBiome;
    internal bool hasKilledTortoise;

    public override void PreUpdate()
    {
        if (!wentThroughNight && !Main.IsItDay()) wentThroughNight = true;
        if (!beenToEvilBiome && (Player.ZoneCorrupt || Player.ZoneCrimson)) beenToEvilBiome = true;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(wentThroughNight)] = wentThroughNight;
        tag[nameof(beenToEvilBiome)] = beenToEvilBiome;
        tag[nameof(hasKilledTortoise)] = hasKilledTortoise;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(wentThroughNight))) wentThroughNight = tag.GetBool(nameof(wentThroughNight));
        if (tag.ContainsKey(nameof(beenToEvilBiome))) beenToEvilBiome = tag.GetBool(nameof(beenToEvilBiome));
        if (tag.ContainsKey(nameof(hasKilledTortoise))) hasKilledTortoise = tag.GetBool(nameof(hasKilledTortoise));
    }
}

internal class ToadShopConditionsNPC : GlobalNPC
{
    public override void HitEffect(NPC npc, NPC.HitInfo hit)
    {
        base.HitEffect(npc, hit);

        if ((npc.type == NPCID.GiantTortoise || npc.type == NPCID.IceTortoise) && npc.life <= 0) Main.LocalPlayer.GetModPlayerOrNull<ToadShopConditionsPlayer>()?.hasKilledTortoise = true;
    }
}