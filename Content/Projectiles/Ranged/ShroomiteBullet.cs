using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;

namespace Project165.Content.Projectiles.Ranged
{
    public class ShroomiteBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(5);
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 800;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 16;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 30f && Projectile.ai[1] == 0f && Main.myPlayer == Projectile.owner) 
            {
                Projectile.ai[1] = -1f;
                Vector2 newVelocity = Vector2.Normalize(Vector2.One * -100f);
                newVelocity = Vector2.Normalize(newVelocity + Vector2.Normalize(Projectile.velocity) * 2f);
                newVelocity *= Projectile.velocity.Length();
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, newVelocity, Type, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0f, -1f);
            }

            if (Projectile.alpha < 127)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust trailDust = Dust.NewDustDirect(Projectile.Center, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 100, new(100, 100, 255), 0.25f);
                    trailDust.position = Projectile.Center - Projectile.velocity / 10f * i;
                    trailDust.noGravity = true;
                    trailDust.velocity *= 0f;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                Dust trailDust = Dust.NewDustDirect(Projectile.position, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 100, new(100, 100, 255), 1f);
                trailDust.noGravity = true;
                trailDust.velocity *= 4f;
            }

            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D trailTexture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color drawColorTrail = Color.SkyBlue with { A = 64 };
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            /*for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 newDrawPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                drawColorTrail *= 0.9f;
                Main.EntitySpriteDraw(trailTexture, newDrawPos, null, drawColorTrail, Projectile.rotation + MathHelper.PiOver2, trailTexture.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            }*/

            Main.EntitySpriteDraw(texture, drawPos, null, drawColor, Projectile.rotation, texture.Size(), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
