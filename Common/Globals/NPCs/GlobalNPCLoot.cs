using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.Weapons.Ranged;
using Project165.Content.Items.Weapons.Summon;
using Project165.Content.Items.Mounts;
using System.Linq;
using Project165.Content.Items.Weapons.Melee;


namespace Project165.Common.Globals.NPCs;

public class GlobalNPCLoot : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        switch (npc.type)
        {
            case NPCID.MartianSaucerCore:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GravityKnives>(), 6));
                break;
            case NPCID.BlueJellyfish:
            case NPCID.GreenJellyfish:
            case NPCID.PinkJellyfish:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyfishStaff>(), 10));
                break;
            case NPCID.SnowBalla:
            case NPCID.SnowmanGangsta:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SnowMount>(), 20));
                break;
            case NPCID.MisterStabby:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SnowMount>(), 20));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AssassinKnife>(), 4));
                break;
        }
    }
}
