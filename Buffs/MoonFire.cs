using Terraria;
using Terraria.ModLoader;
using Project165.Common.Globals.NPCs;

namespace Project165.Buffs
{
    public class MoonFire : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GlobalNPCBuff>().moonFire = true;
        }
    }
}
