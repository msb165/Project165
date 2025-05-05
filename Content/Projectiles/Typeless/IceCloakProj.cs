using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Buffs;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Typeless
{
    internal class IceCloakProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.IceBolt}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(10);
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 900;
        }

        public override void AI()
        {
            float maxDistance = 500f;
            bool foundTarget = false;
            int attackTarget = -1;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (attackTarget == -1 && npc.CanBeChasedBy() && Projectile.Distance(npc.Center) < maxDistance)
                {
                    maxDistance = Projectile.Distance(npc.Center);
                    attackTarget = npc.whoAmI;
                    foundTarget = true;
                }
            }

            if (foundTarget && attackTarget != -1)
            {
                Vector2 speed = Vector2.Normalize(Main.npc[attackTarget].Center - Projectile.Center);
                Projectile.velocity = (Projectile.velocity * 29f + speed * 8f) / 30f;
                double radians = speed.ToRotation() - Projectile.velocity.ToRotation();
                if (radians > MathHelper.Pi)
                {
                    radians -= MathHelper.TwoPi;
                }
                if (radians < -MathHelper.Pi)
                {
                    radians += MathHelper.TwoPi;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity.RotatedBy(radians * 0.1f), 0.4f);
            }
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() + 0.0025f);

            for (int j = 0; j < 2; j++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0, 0, Scale: 0.75f);
                dust.position = Projectile.Center - Projectile.Size * 1.5f + Main.rand.NextVector2Square(10f, 20f) - Projectile.velocity / 2.5f * j;
                dust.velocity = Projectile.velocity;
                dust.scale = 0.75f;
                dust.noGravity = true;

                Dust dust2 = Dust.NewDustDirect(Projectile.position, 1, 1, DustID.FrostStaff, Projectile.oldVelocity.X * 0.2f, Projectile.oldVelocity.Y * 0.2f, 100, Scale: 0.75f);
                dust2.position = Projectile.Center;
                dust2.velocity *= 0.1f;
                dust2.scale = 1.25f;
                dust2.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(BuffID.Frostburn2, 300);
            if (target.boss)
            {
                return;
            }
            target.AddBuff(ModContent.BuffType<SlowDown>(), 300);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.StardustTowerMark);
            Texture2D texture = TextureAssets.Projectile[ProjectileID.StardustTowerMark].Value;
            Vector2 drawOrigin = texture.Size() / 2;
            Color drawColor = Color.DeepSkyBlue with { A = 0 } * Projectile.Opacity;
            Color trailColor = drawColor;
            Color trailColor2 = Color.White with { A = 0 } * Projectile.Opacity;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                trailColor *= 0.9f;
                trailColor2 *= 0.9f;
                float rotation = Projectile.oldRot[i];
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Projectile.velocity / 10f * i - Main.screenPosition;

                Main.spriteBatch.Draw(texture, trailPos, null, trailColor * 0.2f, rotation, drawOrigin, MathHelper.SmoothStep(0f, Projectile.scale * 0.225f, 1f - 0.02f * i), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, trailPos, null, trailColor * 0.2f, rotation, drawOrigin, MathHelper.SmoothStep(0f, Projectile.scale * 0.35f, 1f - 0.02f * i), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, trailPos, null, trailColor2 * 0.1f, rotation, drawOrigin, MathHelper.SmoothStep(0f, Projectile.scale * 0.475f, 1f - 0.02f * i), SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
