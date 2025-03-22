using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Hostile
{
    public class ShadowWave : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.DD2SquireSonicBoom}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.height = 80;
            Projectile.width = 22;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => Projectile.alpha < 127;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft < 300)
            {
                Projectile.velocity *= 0.98f;
            }
            if (Projectile.alpha < 255)
            {
                Projectile.alpha += 2;
            }
            if (Projectile.alpha > 255)
            {
                Projectile.alpha = 255;
                Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2;
            Color trailColor = new Color(50, 200, 50) * 0.75f * Projectile.Opacity;
            Main.spriteBatch.End();
            BlendState multiplyBlendState = new()
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                ColorDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.SourceAlpha
            };
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, multiplyBlendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                trailColor *= 0.9f;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, trailColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, trailColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, trailColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, trailColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
            }

            float offset = MathF.Cos((float)Main.timeForVisualEffects * MathHelper.TwoPi / 30f);
            Color drawColor = new Color(40, 40, 127, 0) * Projectile.Opacity;
            Color glowColor = new Color(50, 255, 50) * 0.5f * Projectile.Opacity;
            glowColor *= 0.75f + 0.25f * offset;

            for (int i = 0; i < 8; i++)
            {
                Vector2 glowDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * i) * (4f + 1f * offset);
                Main.EntitySpriteDraw(texture, glowDrawPos, null, glowColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            for (int i = 0; i < 8; i++)
            {
                Vector2 glowDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * i) * (4f + 1f * offset);
                Main.EntitySpriteDraw(texture, glowDrawPos, null, drawColor * 0.3f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
