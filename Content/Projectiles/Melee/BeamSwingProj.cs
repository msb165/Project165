using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee
{
    internal class BeamSwingProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Excalibur}";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.aiStyle = ProjAIStyleID.NightsEdge;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.ownerHitCheckDistance = 300f;
            Projectile.usesOwnerMeleeHitCD = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }

        public override void AI()
        {            
            float rotation = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
            Vector2 dustPos = Projectile.Center + rotation.ToRotationVector2() * 84f * Projectile.scale;
            Vector2 dustVel = (rotation + Projectile.ai[0] * MathHelper.PiOver2).ToRotationVector2();
            if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + rotation.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), DustID.TreasureSparkle, dustVel * 1f, 100, Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat() * 0.3f), 1.25f);
                dust2.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                dust2.noGravity = true;
            }
            if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
            {
                Dust dust = Dust.NewDustPerfect(dustPos, 43, dustVel * 1f, 100, Color.White * Projectile.Opacity, 1.2f * Projectile.Opacity);
                dust.noGravity = true;
            }

            Lighting.AddLight(Projectile.position, Color.Yellow.ToVector3());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle rectangle = texture.Frame(1, 4);
            Vector2 origin = rectangle.Size() / 2f;
            float scale = Projectile.scale * 1.1f;
            SpriteEffects effects = (!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float num2 = Projectile.localAI[0] / Projectile.ai[1];
            float num3 = Utils.Remap(num2, 0f, 0.6f, 0f, 1f) * Utils.Remap(num2, 0.6f, 1f, 1f, 0f);
            float fromValue = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            fromValue = Utils.Remap(fromValue, 0.2f, 1f, 0f, 1f);

            Color backColor = new(255, 190, 0);
            Color middleColor = new(255, 210, 90);
            Color frontColor = new(255, 255, 202);

            Color color4 = Color.White * num3 * 0.5f;
            color4.A = (byte)(color4.A * (1f - fromValue));
            Color color5 = color4 * fromValue * 0.5f;
            color5.G = (byte)(color5.G * fromValue);
            color5.B = (byte)(color5.R * (0.25f + fromValue * 0.75f));

            Main.spriteBatch.Draw(texture, drawPos, rectangle, backColor * fromValue * num3, Projectile.rotation + Projectile.ai[0] * MathHelper.PiOver4 * -1f * (1f - num2), origin, scale, effects, 0f);
            Main.spriteBatch.Draw(texture, drawPos, rectangle, color5 * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, effects, 0f);
            Main.spriteBatch.Draw(texture, drawPos, rectangle, frontColor * fromValue * num3 * 0.3f, Projectile.rotation, origin, scale, effects, 0f);
            Main.spriteBatch.Draw(texture, drawPos, rectangle, middleColor * fromValue * num3 * 0.5f, Projectile.rotation, origin, scale * 0.975f, effects, 0f);
            Main.spriteBatch.Draw(texture, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.6f * num3, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, effects, 0f);
            Main.spriteBatch.Draw(texture, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.5f * num3, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, effects, 0f);
            Main.spriteBatch.Draw(texture, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.4f * num3, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, effects, 0f);
            for (float i = 0f; i < 8f; i += 1f)
            {
                float edgeRot = Projectile.rotation + Projectile.ai[0] * i * -MathHelper.TwoPi * 0.025f + Utils.Remap(num2, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0];
                Vector2 drawpos = drawPos + edgeRot.ToRotationVector2() * (texture.Width * 0.5f - 6f) * scale;
                DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos, Color.White with { A = 0 } * num3 * (i / 9f), frontColor, num2, 0f, 0.5f, 0.5f, 1f, edgeRot, new Vector2(0f, Utils.Remap(num2, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
            }
            Vector2 drawpos2 = drawPos + (Projectile.rotation + Utils.Remap(num2, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0]).ToRotationVector2() * (texture.Width * 0.5f - 4f) * scale;
            DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos2, Color.White with { A = 0 } * num3 * 0.5f, frontColor, num2, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(num2, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);
            return false;
        }

        private void DrawPrettyStarSparkle(float Opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D value = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Color color = shineColor with { A = 0 } * Opacity * 0.5f;
            Vector2 origin = value.Size() / 2f;
            Color color2 = drawColor * 0.5f;
            float num = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * num;
            Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * num;
            color *= num;
            color2 *= num;
            Main.EntitySpriteDraw(value, drawpos, null, color, MathHelper.PiOver2 + rotation, origin, vector, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color, 0f + rotation, origin, vector2, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color2, MathHelper.PiOver2 + rotation, origin, vector * 0.6f, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
        }
    }
}
