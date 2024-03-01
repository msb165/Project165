using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using Project165.Content.Dusts;
using System;

namespace Project165.Content.Projectiles.Hostile
{
    public class IceBossProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 18;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.aiStyle = -1;
        }

        public bool ShowLaser => Projectile.ai[0] == 1f;
        public bool drawLaser = true;
        public ref float AITimer => ref Projectile.ai[1];

        public override void AI()
        {
            if (Projectile.ai[2] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item28 with { Volume = 0.35f }, Projectile.position);
                Projectile.ai[2] = 1f;
            }            

            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.position, 0f, 0.75f, 1f);

            if (!ShowLaser)
            {
                drawLaser = false;
                return;
            }
            if (Projectile.alpha > 1f)
            {
                Projectile.alpha--;
            }
            AITimer++;
            if (AITimer > 60f)
            {
                Projectile.velocity *= 1.025f;
            }

            drawLaser = AITimer < 120f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float glowScale = 0.5f;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D textureExtra = TextureAssets.Extra[ExtrasID.FairyQueenLance].Value;
            Texture2D glowTexture = (Texture2D)ModContent.Request<Texture2D>("Project165/Assets/Images/GlowSphere");

            Color drawColor = Color.LightCyan with { A = 0 };
            Color drawColorTrail = new(0, 200, 255, 0);            
            Color drawColorTelegraph = new(0, 200, 255, 0);            

            Vector2 drawOrigin = texture.Frame().Size() / 2f;
            Vector2 drawOriginGlow = glowTexture.Frame().Size() / 2f;
            Vector2 drawOriginTelegraph = textureExtra.Frame().Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            if (drawLaser)
            {                
                spriteBatch.Draw(textureExtra, Projectile.Center - Main.screenPosition, null, drawColorTelegraph, Projectile.rotation, drawOriginTelegraph, new Vector2(10f, 3f), SpriteEffects.None, 0);
            }          

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPosTrail = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                drawColorTrail *= 0.75f;
                drawColor *= 0.75f;

                spriteBatch.Draw(glowTexture, drawPosTrail, null, drawColorTrail, Projectile.rotation, drawOriginGlow, glowScale - i / Projectile.oldPos.Length, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPosTrail, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale - i / (float)Projectile.oldPos.Length, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, drawPos, texture.Frame(), drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit();
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, 0, Color.LightCyan, 1f);
            }
            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.35f });
        }
    }
}
