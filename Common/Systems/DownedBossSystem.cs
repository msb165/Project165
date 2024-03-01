using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Project165.Common.Systems
{
    public class DownedBossSystem : ModSystem
    {
        public static bool downedFrigus;
        public override void ClearWorld()
        {
            downedFrigus = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedFrigus)
            {
                tag["downedFrigus"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedFrigus = tag.ContainsKey("downedFrigus");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new(downedFrigus);
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedFrigus = flags[0];
        }
    }
}
