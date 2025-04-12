using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic;

public class AdamantiteBolt : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new(20);
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.aiStyle = -1;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 150;
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 0f)
        {
            Projectile.ai[0] = 1f;
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), newColor: new Color(255, 64, 64), Scale: 1f);
                dust.noGravity = true;
                dust.velocity = (Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 20f) * new Vector2(0.5f, 1.5f)).RotatedBy(Projectile.velocity.ToRotation());
            }
        }

        for (int i = 0; i < 3; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), SpeedX: Projectile.velocity.X * 0.1f, SpeedY: Projectile.velocity.Y * 0.1f, newColor: new Color(255, 64, 64), Scale: 1.25f);
            dust.noGravity = true;
            dust.position = Projectile.Center + Projectile.velocity / 10f * i;
            dust.velocity *= 2f;
        }

        Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());

        Projectile.rotation += 0.25f * Projectile.direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D textureGlow = TextureAssets.Projectile[ProjectileID.StardustTowerMark].Value;
        Main.instance.LoadProjectile(ProjectileID.StardustTowerMark);

        Color drawColor = Color.White * Projectile.Opacity;
        Color trailColor = drawColor;
        Color trailColorGlow = Color.Red with { A = 0 } * 0.5f * Projectile.Opacity;

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            trailColor *= 0.75f;
            trailColorGlow *= 0.75f;
            Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, trailColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, trailColor with { A = 0 }, Projectile.rotation, texture.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None);
            Main.EntitySpriteDraw(textureGlow, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, trailColorGlow, Projectile.rotation, textureGlow.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None);
        }

        //Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        if (Projectile.owner == Main.myPlayer)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 newVelocity = (Vector2.UnitX * 8f).RotatedBy(i * MathHelper.TwoPi / 10f);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, newVelocity, ModContent.ProjectileType<AdamantiteEnergy>(), (int)(Projectile.damage / 3.5f), Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
