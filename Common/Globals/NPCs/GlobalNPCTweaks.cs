using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Common.Globals.NPCs
{
    public class GlobalNPCTweaks : GlobalNPC
    {
        public override void SetDefaults(NPC entity)
        {
            if (!NPC.downedMoonlord)
            {
                return;
            }

            if (!NPCID.Sets.CountsAsCritter[entity.type] && !entity.immortal && !entity.townNPC && entity.type >= NPCID.None && entity.type <= NPCID.Count && !entity.boss)
            {
                float def = MathF.Ceiling(entity.defense * 1.5f);
                entity.defense = (int)def;
                entity.knockBackResist *= 0.75f;
                entity.lifeMax *= 2;
            }
        }
    }
}
