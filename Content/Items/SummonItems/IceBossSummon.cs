using Microsoft.Xna.Framework;
using Project165.Content.NPCs.Bosses.Frigus;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.SummonItems
{
    internal class IceBossSummon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = false;
            Item.maxStack = 1;
        }

        public override bool CanUseItem(Player player)
        {
            if (!Main.IsItDay() && !NPC.AnyNPCs(ModContent.NPCType<IceBossFly>()) && player.ZoneSnow)
            {
                return true;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<IceBossFly>()))
            {
                return false;
            }


            Main.NewText("[c/50F8FF:Looks like nothing happened. It seems that it can only be used in the snow at night time.]");
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(player.position, player.width, player.height, DustID.Electric, 0, 0, 100, default, 1f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = player.Center + new Vector2(0f, -200f) + Main.rand.NextVector2Circular(50f, 50f);
                    NPC.SpawnBoss((int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<IceBossFly>(), player.whoAmI);
                    //NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<IceBossFly>());
                }
                else
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, player.whoAmI, ModContent.NPCType<IceBossFly>());
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FrostCore, 1)
                .AddIngredient(ItemID.IceBlock, 40)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();                
        }
    }
}
