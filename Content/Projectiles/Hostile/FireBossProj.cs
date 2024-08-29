using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Hostile
{
    public class FireBossProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Flamelash}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, Scale: 3f);
                    dust.velocity *= 4f;
                    dust.noGravity = true;
                }
            }
            Lighting.AddLight(Projectile.position + Projectile.velocity, 1f, 0.5f, 0.1f);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            GetFrame();
        }

        public void GetFrame()
        {
            Projectile.frameCounter += 2;
            if (++Projectile.frameCounter >= Main.projFrames[Type] + 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Type];

            int startY = frameHeight * Projectile.frame;

            Color drawColor = Color.White * Projectile.Opacity;
            Color trailColor = drawColor;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                trailColor *= 0.8f;
                Main.EntitySpriteDraw(texture, trailPos, sourceRectangle, trailColor, Projectile.rotation, sourceRectangle.Size() / 2, Projectile.scale - i / (float)Projectile.oldPos.Length, SpriteEffects.None);
                Main.EntitySpriteDraw(texture, trailPos, sourceRectangle, trailColor * 0.2f, Projectile.rotation, sourceRectangle.Size() / 2, (Projectile.scale * 1.75f) - i / (float)Projectile.oldPos.Length, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, drawColor, Projectile.rotation, sourceRectangle.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
