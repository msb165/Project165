using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using Project165.Utilites;

namespace Project165.Content.Projectiles.Melee;

public class RoquefortiProjectile : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 400f;
        ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16.5f;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new(24);
        Projectile.scale = 1f;
        Projectile.aiStyle = ProjAIStyleID.Yoyo;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.MeleeNoSpeed;
        Projectile.noEnchantmentVisuals = true;
    }

    public override void PostAI()
    {
        if (Main.rand.NextBool(3))
        {
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor:Color.RoyalBlue, Scale:3f);
        }

        if (Projectile.owner == Main.myPlayer) 
        {
            Projectile.localAI[1] += 1f;

            if (Projectile.localAI[1] >= 8f)
            {
                float newVelocity = MathHelper.TwoPi + Projectile.localAI[0];

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, newVelocity.ToRotationVector2() * 6f, ModContent.ProjectileType<MushroomProj>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                Projectile.localAI[1] = 0f;
            }                
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D extraThingTexture = (Texture2D)ModContent.Request<Texture2D>(Project165Utils.ImagesPath + "RoquefortiExtra");
        Texture2D glowTexture = (Texture2D)ModContent.Request<Texture2D>(Project165Utils.ImagesPath + "GlowSphere");
        Color drawColor = Color.White with { A = 0 } * Projectile.Opacity; 

        SpriteBatch spriteBatch = Main.spriteBatch;

        Vector2 drawOrigin = texture.Size() / 2f;
        Vector2 drawOriginGlow = glowTexture.Size() / 2f;
        Vector2 thingDrawOrigin = extraThingTexture.Size() / 2f;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        spriteBatch.Draw(extraThingTexture, drawPos, null, drawColor, Projectile.rotation, thingDrawOrigin, 1f, SpriteEffects.None, 0);
        spriteBatch.Draw(glowTexture, drawPos, null, new Color(0, 0, 255, 0), Projectile.rotation, drawOriginGlow, 1f, SpriteEffects.None, 0);
        return false;
    }
}
