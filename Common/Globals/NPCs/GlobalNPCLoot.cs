using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.Weapons.Ranged;
using Project165.Content.Items.Weapons.Summon;
using Project165.Content.Items.Mounts;
using System.Linq;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Magic;


namespace Project165.Common.Globals.NPCs;

public class GlobalNPCLoot : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.ArmoredSkeleton)
        {
            foreach (var rule in npcLoot.Get())
            {
                if (rule is CommonDrop drop && drop.itemId == ItemID.BeamSword)
                {
                    drop.chanceDenominator = 3;
                }
            }
        }

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
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SnowMount>(), 30));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IceFlask>(), 8));
                break;
            case NPCID.SnowmanGangsta:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SnowMount>(), 30));
                break;
            case NPCID.MisterStabby:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SnowMount>(), 30));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AssassinKnife>(), 8));
                break;
            case NPCID.IceElemental:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IceElementalStaff>(), 3));
                break;
            case NPCID.ChatteringTeethBomb:
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsBloodMoonAndNotFromStatue(), ItemID.BloodMoonStarter, 20));
                break;
        }
    }
}
