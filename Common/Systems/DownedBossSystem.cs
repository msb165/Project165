using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Project165.Common.Systems
{
    public class DownedBossSystem : ModSystem
    {
        public static bool downedFrigus;
        public static bool downedShadowHand;
        public static bool downedFireBoss;
        public override void ClearWorld()
        {
            downedFrigus = false;
            downedShadowHand = false;
            downedFireBoss = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedFrigus)
            {
                tag["downedFrigus"] = true;
            }
            if (downedShadowHand)
            {
                tag["downedShadowSlime"] = true;
            }
            if (downedFireBoss)
            {
                tag["downedFireBoss"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedFrigus = tag.ContainsKey("downedFrigus");
            downedShadowHand = tag.ContainsKey("downedShadowSlime");
            downedFireBoss = tag.ContainsKey("downedFireBoss");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new(downedFrigus, downedShadowHand, downedFireBoss);
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedFrigus = flags[0];
            downedShadowHand = flags[1];
            downedFireBoss = flags[2];
        }
    }
}
