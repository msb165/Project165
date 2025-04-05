using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Common.Globals.NPCs
{
    public class GlobalNPCTweaks : GlobalNPC
    {
        static List<int> excludedNPCS = [NPCID.DungeonGuardian, NPCID.MartianProbe, NPCID.CultistDevote, NPCID.EmpressButterfly, NPCID.Bee];
        public override void SetDefaults(NPC entity)
        {
            if (!NPC.downedMoonlord)
            {
                return;
            }

            bool validEntity = !NPCID.Sets.CountsAsCritter[entity.type] && !entity.friendly && !entity.immortal && !entity.townNPC && entity.type >= NPCID.None && entity.type <= NPCID.Count && !entity.boss;
            if (validEntity && !excludedNPCS.Contains(entity.type))
            {
                float def = MathF.Ceiling(entity.defense * 1.5f);
                entity.defense = (int)def;
                entity.knockBackResist *= 0.75f;
                entity.lifeMax *= 2;
            }
        }
    }
}
