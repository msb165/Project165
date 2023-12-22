using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Project165.Common.Globals
{
    internal class GlobalNPCSpawn : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (Main.hardMode && spawnInfo.Player.ZoneForest)
            {
                pool[NPCID.SlimeSpiked] = SpawnCondition.OverworldDay.Chance * 0.25f;
            }

            if (Main.hardMode && spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneOverworldHeight)
            {
                pool[NPCID.IceGolem] = SpawnCondition.Overworld.Chance * 0.25f;
            }
        }
    }
}
