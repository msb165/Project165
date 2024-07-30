using Project165.Content.Items.Materials;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Project165.Content.NPCs.Enemies
{
    public class DarkSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.Slime;
            NPC.alpha = 127;
            NPC.width = 44;
            NPC.height = 34;
            NPC.damage = 13;
            NPC.lifeMax = 180;
            NPC.value = 50f;
            AnimationType = NPCID.BlueSlime;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(
            [
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface
            ]);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowGel>(), 1, 1, 5));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => NPC.downedGolemBoss ? SpawnCondition.OverworldNight.Chance * 0.4f : 0f;
    }
}
