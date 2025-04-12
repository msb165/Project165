using Project165.Content.NPCs.Enemies;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.ILEdits;

public partial class DetourChanges : ModSystem
{
    private int On_NPC_GetNPCInvasionGroup(On_NPC.orig_GetNPCInvasionGroup orig, int npcID)
    {
        int result = orig(npcID);
        if (npcID == ModContent.NPCType<TallSnowman>())
        {
            result = InvasionID.SnowLegion;
        }
        return result;
    }
}
