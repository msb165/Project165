using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Ranged;

public class InfernalArrow : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new(16);
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.arrow = true;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        //Projectile.velocity.Y += 0.2f;
        if (Projectile.velocity.Y > 16f)
        {
            //Projectile.velocity.Y = 16f;
        }
        Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3());

        for (int i = 0; i < 3; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.Orange, Scale: 1f);
            dust.noGravity = true;
            dust.velocity *= 0.3f;
            dust.position = Projectile.Center - Projectile.velocity / 5f * i;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.myPlayer == Projectile.owner && !NPCID.Sets.CountsAsCritter[target.type] && target.chaseable && !target.immortal)
        {
            Vector2 spawnPos = Main.rand.NextVector2CircularEdge(200f, 200f);
            Vector2 spawnVel = spawnPos.SafeNormalize(spawnPos) * 8f;

            if (spawnVel.Y > 0f)
            {
                spawnVel.Y *= -1f;
            }

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center - spawnVel * 10f, spawnVel, ModContent.ProjectileType<InfernalFire>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
        }
        target.AddBuff(BuffID.OnFire3, 300);
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 30; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.Orange, Scale: 1f);
            dust.noGravity = true;
            dust.velocity *= 4f;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D fireTexture = TextureAssets.Extra[ExtrasID.FallingStar].Value;
        Texture2D arrowTexture = TextureAssets.Projectile[Type].Value;
        Color drawColor = Projectile.GetAlpha(lightColor);
        Color trailColor = Color.Yellow with { A = 0 } * Projectile.Opacity;
        Color fireColor = Color.Orange with { A = 0 } * Projectile.Opacity;

        Vector2 drawOrigin = new(arrowTexture.Width / 2, 0f);
        Vector2 drawOriginFire = fireTexture.Width * Vector2.UnitX / 2;

        float timer = (float)(Main.timeForVisualEffects / 60.0) % 0.5f / 0.5f;

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            trailColor *= 0.75f;
            Main.EntitySpriteDraw(arrowTexture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, trailColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        }

        for (float j = 0; j < 1f; j += 0.5f)
        {
            timer = (timer + j) % 1f;
            float doubleTimer = timer * 2f;
            if (doubleTimer > 1f)
            {
                doubleTimer = 2f - doubleTimer;
            }
            Main.EntitySpriteDraw(fireTexture, Projectile.Center - Main.screenPosition, null, fireColor * doubleTimer, Projectile.rotation, drawOriginFire, 0.75f + timer * 0.5f, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(arrowTexture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);

        return false;
    }
}
