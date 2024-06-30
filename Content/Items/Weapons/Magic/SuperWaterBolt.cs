using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Magic;


namespace Project165.Content.Items.Weapons.Magic
{
    public class SuperWaterBolt : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.damage = 38;
            Item.mana = 10;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.UseSound = SoundID.Item43;
            Item.shoot = ModContent.ProjectileType<SuperWaterBoltProj>();
            Item.shootSpeed = 12f;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 newPosition = player.Center + new Vector2(-Main.rand.Next(50, 101) * player.direction, Main.rand.Next(-100, 51));
            Vector2 newVelocity = Vector2.Normalize(Main.MouseWorld - newPosition) * Item.shootSpeed;

            Projectile.NewProjectileDirect(source, newPosition, newVelocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1);
        }
    }
}
