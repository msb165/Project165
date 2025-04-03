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
            //Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 58;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 120;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
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
            player.itemRotation += MathHelper.PiOver4 * -player.direction;
            player.itemLocation = player.Center + Vector2.One;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 newVel = new Vector2(4f * player.direction, -12f);
                Projectile.NewProjectile(source, new Vector2(position.X, player.Center.Y - player.height) + newVel * 8f, newVel.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(1f, 1.5f), type, damage, knockback, player.whoAmI);
            }

            return false;
        }
    }
}
