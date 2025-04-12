using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Ranged;

public class GolemArrow : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new(16);
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.friendly = true;
        Projectile.arrow = true;
        Projectile.aiStyle = ProjAIStyleID.Arrow;
    }

    public override void OnKill(int timeLeft)
    {
        Projectile.Resize(64, 64);
        SoundEngine.PlaySound(SoundID.Item14);
        for (int i = 0; i < 30; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, -Projectile.oldVelocity.X * 0.2f, -Projectile.oldVelocity.Y * 0.2f, newColor: Color.Yellow);
            dust.velocity *= 2f;
            dust.noGravity = true;
            Dust newDust = Dust.CloneDust(dust);
            newDust.velocity *= 0.5f;
            newDust.color = Color.OrangeRed;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = Vector2.UnitX * texture.Width / 2;

        Color drawColor = Color.White * Projectile.Opacity;
        Color startColor = Color.OrangeRed with { A = 0 } * Projectile.Opacity;
        Color endColor = Color.Yellow with { A = 0 } * Projectile.Opacity;

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Color trailColor = Color.Lerp(startColor, endColor, 1f - i * 0.05f) * Utils.GetLerpValue(1f, 0f, i * 0.04f, true);
            Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, texture.Frame(), trailColor, Projectile.oldRot[i], drawOrigin, Utils.GetLerpValue(Projectile.scale * 1.125f, 0f, i * 0.01f, true), SpriteEffects.None);
        }

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);

        return false;
    }
}
