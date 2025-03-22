using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Ranged
{
    public class InfernalBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 62;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.damage = 55;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.InfernalBow>();
            Item.shootSpeed = 20f;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ProjectileID.HellfireArrow;
            }

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Ranged.InfernalBow>(), damage, knockback, player.whoAmI, ai2: type);
            return false;
        }
    }
}
