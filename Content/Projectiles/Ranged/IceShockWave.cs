using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Project165.Content.Dusts;

namespace Project165.Content.Projectiles.Ranged
{
    public class IceShockWave : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.DD2SquireSonicBoom}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 100;
            Projectile.extraUpdates = 1;
        }


        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            for (int i = 0; i < 15; i++)
            {
                Vector2 spinPosition = new Vector2(10f, 40f).RotatedBy(Projectile.velocity.ToRotation());
                Dust dust = Dust.NewDustDirect(Projectile.position, 0, 0, DustID.IceTorch, 0, 0, 100, default, 1.5f);
                dust.velocity *= 0.5f;
                dust.position = Projectile.Center - Projectile.velocity / 10f * i + spinPosition;
                dust.noGravity = true;
                Vector2 spinPosition2 = new Vector2(10f, -40f).RotatedBy(Projectile.velocity.ToRotation());
                Dust dust2 = Dust.CloneDust(dust);
                dust2.position = Projectile.Center - Projectile.velocity / 10f * i + spinPosition2;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(-MathHelper.PiOver2) * Projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - offset * 40f, Projectile.Center + offset * 40f, 16f * Projectile.scale, ref _);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.IceTorch, 0, 0, 0, default, 2f);
                dust.noGravity = true;
                dust.velocity *= 8f;
                dust.position += dust.velocity * 4f;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2;
            Color drawColor = Color.SkyBlue with { A = 127 } * Projectile.Opacity;
            Color trailColor = drawColor with { A = 0 };

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                trailColor *= 0.75f;
                Main.EntitySpriteDraw(texture, oldPos, texture.Frame(), trailColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
