using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class IceFlaskProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(18);
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            float rotationValue = Projectile.velocity.X * 0.04f;
            Projectile.rotation += MathHelper.Lerp(0f, rotationValue, 0.8f) * Projectile.direction;
            Projectile.velocity.Y += 0.2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.HasBuff(ModContent.BuffType<SlowDown>()) && !target.boss)
            {
                target.AddBuff(ModContent.BuffType<SlowDown>(), 100);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item107, Projectile.position);

            if (Main.netMode != NetmodeID.Server)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, GoreID.ToxicFlask);
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, GoreID.ToxicFlask2);
            }

            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(2, 6); i++)
                {
                    Vector2 targetVel = Utils.RandomVector2(Main.rand, -100, 101).SafeNormalize(Vector2.UnitY) * Main.rand.Next(7, 10);
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.oldPosition + Projectile.Size / 2, targetVel, ModContent.ProjectileType<IceFlaskBolt>(), Projectile.damage, 0f, Projectile.owner);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color drawColorTrail = drawColor;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                drawColorTrail *= 0.75f;
                spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, texture.Frame(), drawColorTrail, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
