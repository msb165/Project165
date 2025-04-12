using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee;

public class ShadowBall : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new(24);
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.MeleeNoSpeed;
        Projectile.aiStyle = -1;
        Projectile.noEnchantmentVisuals = true;
    }

    Player Owner => Main.player[Projectile.owner];
    public int YoYoIndex => (int)Projectile.ai[0];

    public override void AI()
    {
        float rad = MathHelper.ToRadians(Projectile.ai[1] * 10f) * Owner.direction;
        Projectile.ai[1]++;

        Projectile.spriteDirection = Projectile.direction;

        // This handles the projectiles' life
        Projectile.penetrate = (int)Main.projectile[YoYoIndex].ai[1];
        Projectile.Center = Main.projectile[YoYoIndex].Center;
        Projectile.position = Main.projectile[YoYoIndex].Center - (rad.ToRotationVector2() * 90f) - Projectile.Size / 2;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.ShadowFlame, 300);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D extraThingTexture = (Texture2D)ModContent.Request<Texture2D>("Project165/Assets/Images/RoquefortiExtra");
        Color trailColor = Color.Black with { A = 100, R = 64, B = 64 };

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            trailColor *= 0.85f;
            Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, texture.Frame(), trailColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }

        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        Main.spriteBatch.Draw(extraThingTexture, Projectile.Center - Main.screenPosition, extraThingTexture.Frame(), Color.Purple with { A = 0 }, Projectile.rotation, extraThingTexture.Size() / 2, 0.75f, SpriteEffects.None, 0);
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Black with { A = 32 }, Projectile.rotation, texture.Size() / 2, Projectile.scale * 1.25f, SpriteEffects.None, 0);

        return false;
    }
}
