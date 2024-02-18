using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Ranged
{
    internal class IceShotgun : ModItem
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
            Item.UseSound = SoundID.Item36;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 22;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int m = 0; m < 30; m++)
            {
                Vector2 offset = Vector2.UnitX.RotatedBy(velocity.ToRotation());
                Vector2 spawnPos = offset.RotatedByRandom(0.3f);
                Dust.NewDustPerfect(player.Center + spawnPos * 50, ModContent.DustType<GlowDust>(), (spawnPos * Main.rand.NextFloat(0.9f, 1.2f)) - Vector2.UnitY, 100, Color.LightCyan, 0.75f);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15f));

                newVelocity *= 1f - Main.rand.NextFloat(0.4f);

                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback);
                Projectile.NewProjectileDirect(source, position, newVelocity * 4f, ProjectileID.CrystalShard, damage / 3, knockback);
                Projectile.NewProjectileDirect(source, position, newVelocity * 4f, ProjectileID.CrystalShard, damage / 3, knockback);
            }

            return false;
        }

        public override Vector2? HoldoutOffset() => new(-1.25f);
    }
}
