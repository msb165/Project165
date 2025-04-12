using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic;

public class ShadowGas : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.Size = new(32);
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.scale = 1.1f;
        Projectile.DamageType = DamageClass.Magic;
    }

    public override void AI()
    {
        Vector2 slowVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.05f;

        Projectile.localAI[0]++;
        Projectile.rotation = Projectile.whoAmI * 0.4f + Projectile.localAI[0] * MathHelper.TwoPi * 0.005f;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity - slowVelocity, 0.6f);
        if (Projectile.alpha < 255)
        {
            Projectile.alpha++;
        }
        if (Projectile.alpha >= 255)
        {
            Projectile.Kill();
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.ShadowFlame, 300);
        target.AddBuff(BuffID.Venom, 300);
    }


    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Vector2 origin = texture.Size() / 2;
        Color drawColor = Color.White * Projectile.Opacity;

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

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor with { A = 80 }, Projectile.rotation, origin, Projectile.scale * 1.5f, SpriteEffects.None);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        return false;
    }
}
