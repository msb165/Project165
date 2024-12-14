using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class WormholeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.Size = new(30);
            Projectile.DamageType = DamageClass.Default;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 500;
            Projectile.penetrate = -1;
        }

        public float maxTime = 500;
        public float AITimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float AICounter
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void AI()
        {
            AITimer++;
            if (AITimer > 24f && AICounter == 0f)
            {
                AITimer = 0f;
                AICounter = 1f;
                Projectile.netUpdate = true;
                Projectile.velocity *= 0f;
            }
            Projectile.rotation += 0.125f * Projectile.direction;

            if (Projectile.timeLeft <= maxTime * 0.125f && Projectile.scale > 0f)
            {
                Projectile.scale -= 0.0125f;
                Projectile.alpha += 4;
            }

            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.LightPink, 0.5f);

            if (Main.myPlayer == Projectile.owner && AITimer % 20f == 0f && AICounter == 1f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-10, -2)), ModContent.ProjectileType<WormholeBolt>(), Projectile.damage, 0f, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), 0, 0, 0, Color.Magenta, 1.25f);
                dust.velocity *= 4f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.instance.LoadProjectile(ProjectileID.StardustTowerMark);
            Texture2D glowTexture = TextureAssets.Projectile[ProjectileID.StardustTowerMark].Value;

            // Shadow at the back
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Black * 0.45f * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale * 2f, SpriteEffects.None, 0);
            // Subtle glow
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Blue with { A = 0 } * 0.2f * Projectile.Opacity, -Projectile.rotation * 0.5f, texture.Size() / 2, Projectile.scale * 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Purple with { A = 0 } * Projectile.Opacity, -Projectile.rotation * 0.75f, texture.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Purple with { A = 0 } * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            // Glow in the middle
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White with { A = 0 } * 0.75f * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            // Glow around the thing
            Main.spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, glowTexture.Frame(), new Color(100, 100, 255, 0) * 0.35f * Projectile.Opacity, Projectile.rotation, glowTexture.Size() / 2, Projectile.scale * 2f, SpriteEffects.None, 0);            
            return false;
        }
    }
}
