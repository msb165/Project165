using Project165.Content.NPCs;
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
            return !Main.IsItDay() && !NPC.AnyNPCs(ModContent.NPCType<IceBossFly>()) && player.ZoneSnow;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<IceBossFly>());
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
