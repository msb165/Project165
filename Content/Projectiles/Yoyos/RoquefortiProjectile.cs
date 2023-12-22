using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Projectiles.Melee;
using System;

namespace Project165.Content.Projectiles.Yoyos
{
    internal class RoquefortiProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 300f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 20f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1.2f;

            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }        

        public override void PostAI()
        {            
            if (Projectile.owner == Main.myPlayer) 
            {
                Projectile.localAI[1] += 1f;

                if (Projectile.localAI[1] >= 8f)
                {
                    float newVelocity = MathHelper.TwoPi + Projectile.localAI[0];

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, newVelocity.ToRotationVector2() * 6f, ModContent.ProjectileType<MushroomProj>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner); ;
                    Projectile.localAI[1] = 0f;
                }
                
            }

            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GlowingMushroom, 0f, 0f, 100, default, 1f);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D extraThingTexture = (Texture2D)ModContent.Request<Texture2D>("Project165/Assets/Images/RoquefortiExtra");
            Color drawColor = Projectile.GetAlpha(lightColor);

            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Vector2 thingDrawOrigin = new(extraThingTexture.Width / 2, extraThingTexture.Height / 2);

            Main.spriteBatch.Draw(extraThingTexture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, thingDrawOrigin, 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
