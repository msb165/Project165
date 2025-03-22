using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Project165.Content.NPCs.Bosses.FireBoss;
using Microsoft.Xna.Framework;

namespace Project165.Content.Items.SummonItems
{
    public class FireBossSummon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Red;
            Item.maxStack = 1;
        }

        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<FireBoss>()) && player.ZoneUnderworldHeight)
            {
                return true;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<FireBoss>()))
            {
                return false;
            }

            Main.NewText("[c/FFFF50:Looks like nothing happened. It seems that it can only be used in the underworld...]");
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Firework_Yellow, Alpha: 100);
                    dust.velocity *= 4f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = player.Center + new Vector2(0f, -200f) + Main.rand.NextVector2Circular(50f, 50f);
                    NPC.SpawnBoss((int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<FireBoss>(), player.whoAmI);
                }
                else
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, player.whoAmI, number: ModContent.NPCType<FireBoss>());
                }
            }
            return true;
        }
    }
}
