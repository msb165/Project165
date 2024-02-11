using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;

namespace Project165.Content.Projectiles.Melee
{
    internal class RoquefortiProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 400f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16.5f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.scale = 1f;

            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }        

        public override void PostAI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), 0f, 0f, 0, Color.RoyalBlue, 3f);
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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            projHitbox.Width = 96;            
            projHitbox.Height = 96;           
            
            return projHitbox.Intersects(targetHitbox);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D extraThingTexture = (Texture2D)ModContent.Request<Texture2D>("Project165/Assets/Images/RoquefortiExtra");
            Texture2D glowTexture = (Texture2D)ModContent.Request<Texture2D>("Project165/Assets/Images/GlowSphere");
            Color drawColor = Projectile.GetAlpha(lightColor);

            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawOriginGlow = glowTexture.Size() / 2f;
            Vector2 thingDrawOrigin = extraThingTexture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            spriteBatch.Draw(extraThingTexture, drawPos, null, drawColor, Projectile.rotation, thingDrawOrigin, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(glowTexture, drawPos, null, new Color(0, 0, 255, 0), Projectile.rotation, drawOriginGlow, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
