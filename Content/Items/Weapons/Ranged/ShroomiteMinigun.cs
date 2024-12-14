using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Ranged
{
    public class ShroomiteMinigun : ModItem
    { 
        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 7;
            Item.useTime = 7;
            Item.width = 60;
            Item.height = 20;
            Item.useAmmo = AmmoID.Bullet;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 30;
            Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.ShroomiteMinigun>();
            Item.shootSpeed = 10f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 35);
            Item.rare = ItemRarityID.Pink;
            Item.knockBack = 1f;
        }

        public override bool CanShoot(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Ranged.ShroomiteMinigun>(), damage, knockback, player.whoAmI);
            return false;
        }

        //public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(2);
        //public override Vector2? HoldoutOffset() => new(1, 6);
    }
}
