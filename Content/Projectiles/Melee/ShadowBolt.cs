using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Project165.Content.Dusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee
{
    public class ShadowBolt : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.WoodenArrowFriendly}";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(14);
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 12;
            Projectile.noEnchantmentVisuals = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), 0f, 0f, 100, Color.DarkSlateBlue, 0.75f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Projectile.velocity.X * 0.75f, Projectile.velocity.Y * 0.75f, 100, Color.DarkSlateBlue, 0.75f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Extra[98].Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColorTrail = Color.DarkSlateBlue with { A = 0 };

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                drawColorTrail *= 0.98f;
                spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, drawColorTrail, Projectile.rotation, texture.Size() / 2, Projectile.scale - i / (float)Projectile.oldPos.Length, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, drawColorTrail * 0.35f, Projectile.rotation, texture.Size() / 2, (Projectile.scale * 1.5f) - i / (float)Projectile.oldPos.Length, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DarkSlateBlue with { A = 0 } * 0.5f, Projectile.rotation, texture.Size() / 2, Projectile.scale * 2f, SpriteEffects.None, 0);
            return false;
        }
    }
}
