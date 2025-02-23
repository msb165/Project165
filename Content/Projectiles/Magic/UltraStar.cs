using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class UltraStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(20);
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f;
            for (int i = 0; i < 3; i++)
            {
                Dust starless = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Yellow, Scale:0.5f);
                starless.position = Projectile.Center - Projectile.velocity / 10f * i;
                starless.noGravity = true;
                starless.velocity *= 1.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color trailColor = Projectile.GetAlpha(lightColor) with { A = 0 };
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation;
            float scale = Projectile.scale;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                scale *= 1.0625f;
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                trailColor *= 0.75f;
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, Projectile.rotation, drawOrigin, scale, spriteEffects);
            }

            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), drawColor with { A = 0 }, rotation, drawOrigin, Projectile.scale * 1.25f, spriteEffects);
            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), drawColor, rotation, drawOrigin, Projectile.scale, spriteEffects);
            return false;
        }
    }
}
