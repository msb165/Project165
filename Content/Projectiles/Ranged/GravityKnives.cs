using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using Project165.Content.Items.Weapons.Ranged;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Ranged
{
    public class GravityKnives : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<Items.Weapons.Ranged.GravityKnives>().Texture; 
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y -= 0.1f;
            if (Projectile.velocity.Y < -16f)
            {
                Projectile.velocity.Y = -16f;
            }

            for (int i = 0; i < 10; i++)
            {
                Vector2 spinPoint = -Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 10f) * new Vector2(1f, 4f);
                spinPoint = spinPoint.RotatedBy(Projectile.velocity.ToRotation());
                Dust circleDust = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, Vector2.Zero, 0, default, 0.7f);
                circleDust.noGravity = true;
                circleDust.position = Projectile.position + Projectile.Size / 2 + spinPoint;
                circleDust.velocity = spinPoint.SafeNormalize(Vector2.UnitY) * 0.5f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Cyan, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 0, default, 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D extraTexture = (Texture2D)ModContent.Request<Texture2D>("Project165/Assets/Images/GravityKnivesExtra");
            Vector2 origin = Vector2.UnitX * texture.Width / 2f;
            Color trailColor = Color.SkyBlue with { A = 0 } * Projectile.Opacity;
            Color glowColor = trailColor;

            for (int i = 1; i <= Projectile.oldPos.Length - 1; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                trailColor *= 0.85f;
                Main.EntitySpriteDraw(extraTexture, oldPos, texture.Frame(), trailColor * 0.75f, Projectile.rotation, origin, Projectile.scale - i / (float)Projectile.oldPos.Length, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), glowColor * 0.5f, Projectile.rotation, origin, Projectile.scale * 1.25f, SpriteEffects.None, 0);

            return false;
        }
    }
}
