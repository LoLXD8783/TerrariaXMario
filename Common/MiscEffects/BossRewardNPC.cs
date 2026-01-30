using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Content.Materials;

namespace TerrariaXMario.Common.MiscEffects;

internal class BossRewardNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        IItemDropRule rule = ItemDropRule.Common(ModContent.ItemType<RootBean>(), minimumDropped: Main.BestiaryTracker.Kills.GetKillCount(npc) == 0 ? (Main.hardMode ? 3 : 1) : 1, maximumDropped: Main.BestiaryTracker.Kills.GetKillCount(npc) == 0 ? (Main.hardMode ? 5 : 3) : 1);

        if (System.Array.IndexOf([NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail], npc.type) > -1)
        {
            LeadingConditionRule leadingConditionRule = new(new Conditions.LegacyHack_IsABoss());
            leadingConditionRule.OnSuccess(rule);
            npcLoot.Add(leadingConditionRule);
        }
        else if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
        {
            LeadingConditionRule leadingConditionRule = new(new Conditions.MissingTwin());
            leadingConditionRule.OnSuccess(rule);
            npcLoot.Add(leadingConditionRule);
        }
        else if (npc.boss) npcLoot.Add(rule);

        base.ModifyNPCLoot(npc, npcLoot);
    }
}
