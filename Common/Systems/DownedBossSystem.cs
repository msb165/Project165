using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Project165.Common.Systems
{
    public class DownedBossSystem : ModSystem
    {
        public static bool downedFrigus;
        public static bool downedShadowSlime;
        public override void ClearWorld()
        {
            downedFrigus = false;
            downedShadowSlime = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedFrigus)
            {
                tag["downedFrigus"] = true;
            }
            if (downedShadowSlime)
            {
                tag["downedShadowSlime"] = true;
            }            
            
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedFrigus = tag.ContainsKey("downedFrigus");
            downedShadowSlime = tag.ContainsKey("downedShadowSlime");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new(downedFrigus, downedShadowSlime);
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedFrigus = flags[0];
            downedShadowSlime = flags[1];
        }
    }
}
