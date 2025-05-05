using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Ranged
{
    public class HallowedJavelin : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<HallowedJavelinProj>();
            Item.shootSpeed = 10f;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(copper: 10);
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public int shootCount = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(5f));
            Projectile.NewProjectileDirect(source, position + newVel * 4f, newVel, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
                .AddIngredient(ItemID.HallowedBar, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
