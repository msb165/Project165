using Newtonsoft.Json.Linq;
using Project165.Content.Items.Materials;
using Project165.Content.Projectiles.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Magic
{
    // Idea: Weapon that would work like the old rainbow rod but would increase in size upon hitting an enemy and slowly pull some enemies and then the projectile would explode into gas (similar to the toxic flask)
    public class BlackHoleStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.mana = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<BlackHoleProj>();
            Item.shootSpeed = 1f;
            Item.width = 38;
            Item.height = 36;
            Item.UseSound = SoundID.Item43;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 4, silver: 12);
            Item.damage = 90;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player) => !player.channel;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RainbowRod)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ModContent.ItemType<ShadowEssence>(), 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
