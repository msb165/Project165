using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Ranged
{
    public class IceShotgun : ModItem
    {
        public override void SetDefaults()
        {
            Item.useAnimation = 42;
            Item.useTime = 42;
            Item.width = 60;
            Item.height = 18;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Bullet;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item36 with { Pitch = -0.25f };
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 22;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.velocity.X += -3.5f * player.direction;
            for (int m = 0; m < 5; m++)
            {
                Vector2 offset = Vector2.UnitX.RotatedBy(velocity.ToRotation());
                Vector2 spawnPos = offset.RotatedByRandom(0.5f);
                Dust.NewDustPerfect(player.Center + spawnPos * 50f, ModContent.DustType<CloudDust>(), spawnPos - Vector2.UnitY * 4f, 100, default, Main.rand.NextFloat(0.75f, 1.1f));
                Dust.NewDustDirect(player.Center, Item.width, Item.height, ModContent.DustType<CloudDust>(), 0, 0, 100, default, Main.rand.NextFloat(0.75f, 1.1f));
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15f));

                newVelocity *= 1f - Main.rand.NextFloat(0.4f);

                Projectile crystalShard = Projectile.NewProjectileDirect(source, position, newVelocity * 2.5f, ProjectileID.CrystalShard, damage, knockback);
                crystalShard.tileCollide = true;
                crystalShard.scale = 1.75f;
                crystalShard = Projectile.NewProjectileDirect(source, position, newVelocity * 4f, ProjectileID.CrystalShard, damage / 2, knockback);
                crystalShard.penetrate = -1;
            }

            return false;
        }

        public override Vector2? HoldoutOffset() => new(-1.25f);
    }
}
