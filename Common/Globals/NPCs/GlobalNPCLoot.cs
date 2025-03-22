using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.Weapons.Ranged;
using Project165.Content.Items.Weapons.Summon;


namespace Project165.Common.Globals.NPCs
{
    public class GlobalNPCLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.MartianSaucerCore:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GravityKnives>(), 6));
                    break;
                case NPCID.Golem:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GolemBow>(), 8));
                    break;
                case NPCID.BlueJellyfish:
                case NPCID.GreenJellyfish:
                case NPCID.PinkJellyfish:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyfishStaff>(), 10));
                    break;
            }
        }
    }
}
