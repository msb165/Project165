using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Common.Globals.NPCs;

public class GlobalNPCTweaks : GlobalNPC
{
    static List<int> excludedNPCS = [NPCID.DungeonGuardian, NPCID.MartianProbe, NPCID.CultistDevote, NPCID.EmpressButterfly, NPCID.Bee];
    public override void SetDefaults(NPC entity)
    {
        switch (entity.type)
        {
            case NPCID.SnowmanGangsta:
                entity.damage = 20;
                entity.lifeMax = 80;
                entity.defense = 6;
                entity.value = 100f;
                break;
            case NPCID.MisterStabby:
                entity.damage = 25;
                entity.lifeMax = 80;
                entity.defense = 6;
                entity.value = 100f;
                break;
            case NPCID.SnowBalla:
                entity.damage = 22;
                entity.lifeMax = 110;
                entity.defense = 6;
                entity.value = 100f;
                break;
        }

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

    public override bool PreAI(NPC npc)
    {
        if (npc.aiStyle == NPCAIStyleID.Snowman)
        {
            npc.rotation = MathHelper.Lerp(npc.rotation, npc.velocity.X * 0.05f, 0.08f);
            if (npc.velocity.Y == 0f)
            {
                npc.rotation = MathHelper.Lerp(npc.rotation, 0f, 1f);
            }
        }
        return true;
    }
}

