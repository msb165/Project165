using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Common.Globals.NPCs;

public class GlobalNPCShop : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.ArmsDealer)
        {
            shop.Add(ModContent.ItemType<MachineGunStaff>(), Condition.DownedEyeOfCthulhu);
        }
        if (shop.NpcType == NPCID.Truffle)
        {
            shop.Add(ModContent.ItemType<Roqueforti>(), Condition.DownedMechBossAny);
        }
        if (shop.NpcType == NPCID.Dryad)
        {
            shop.Add(ItemID.HerbBag, Condition.DownedEyeOfCthulhu);
        }
    }
}
