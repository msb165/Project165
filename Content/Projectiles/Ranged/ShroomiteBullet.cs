using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project165.Content.Projectiles.Ranged
{
    public class ShroomiteBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 60;
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
            if (Projectile.ai[0] > 30f && Projectile.ai[1] == 0f && Main.myPlayer == Projectile.owner && Main.rand.NextBool(10)) 
            {
                Projectile.ai[1] = -1f;
                Vector2 newVelocity = Vector2.Normalize(Vector2.One * -100f);
                newVelocity = Vector2.Normalize(newVelocity + Vector2.Normalize(Projectile.velocity) * 2f) * Projectile.velocity.Length();
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, newVelocity, Type, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0f, -1f);
            }

            if (Projectile.alpha < 127)
            {
                //Dust trailDust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.BlueTorch, newColor:new(100, 100, 255), Scale:1f);
                Dust trailDust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.MushroomTorch, Scale: 1f);
                trailDust.scale = 1f;
                trailDust.position = Projectile.Center - Projectile.velocity / 10f;
                trailDust.noGravity = true;
                trailDust.velocity *= 2f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                Dust trailDust = Dust.NewDustDirect(Projectile.position, 1, 1, DustID.MushroomTorch, Scale: 1.5f);
                trailDust.noGravity = true;
                trailDust.velocity *= 8f;
            }

            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.FairyQueenMagicItemShot);
            //Texture2D texture = TextureAssets.Projectile[ProjectileID.FairyQueenMagicItemShot].Value;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            float rotation = Projectile.rotation + MathHelper.PiOver2;
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 8);
            Color drawColor = Color.DeepSkyBlue with { A = 0 } * Projectile.Opacity;
            Color drawColorTrail = drawColor;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 10; i++)
            {
                drawColorTrail *= 0.75f;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, drawColorTrail, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            //Main.EntitySpriteDraw(texture, drawPos, null, drawColor, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
