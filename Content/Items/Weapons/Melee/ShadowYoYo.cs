using Microsoft.Xna.Framework;
using Project165.Content.Items.Materials;
using Project165.Content.Projectiles.Melee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Melee
{
    public class ShadowYoYo : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 120;
            Item.knockBack = 2f;
            Item.crit = 8;
            Item.channel = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 0, silver: 80);
            Item.shoot = ModContent.ProjectileType<ShadowYoYoProj>();
            Item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ShadowEssence>(), 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
