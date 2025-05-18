using Project165.Common.Globals.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Buffs
{
    public class SlowDown : ModBuff
    {
        public override string Texture => $"Terraria/Images/Buff_{BuffID.Slow}";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GlobalNPCBuff>().slowDown = true;
        }
    }
}
