using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Magic;
using Project165.Utilites;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Magic
{
    public class UltraFireStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 58;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 120;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item73;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.shoot = ModContent.ProjectileType<BouncyFire>();
            Item.shootSpeed = 15f;
            Item.mana = 25;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(silver: 40, copper: 25);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            Project165Utils.SmoothHoldStyle(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(source, position + velocity * 5f, velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(1f, 1.5f), type, damage, knockback, player.whoAmI);
            }

            return false;
        }
    }
}
