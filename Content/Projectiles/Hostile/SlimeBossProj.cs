using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Hostile
{
    public class SlimeBossProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(34);
            Projectile.aiStyle = -1;
            Projectile.penetrate = 5;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.scale = 1.25f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 100;
        }

        public bool ShouldPlaySound => Projectile.ai[1] == 0f;

        public override void AI()
        {
            if (ShouldPlaySound)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item155, Projectile.position);
            }

            GenerateDust();

            Projectile.rotation += 0.2f * Projectile.direction;
            //Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public void GenerateDust()
        {
            Color newColor = Color.Purple with { A = 0 };
            Dust slimeDust = Dust.NewDustDirect(Projectile.position - Vector2.One * 8 + Projectile.velocity, Projectile.width + 16, Projectile.height + 16, DustID.t_Slime, 0f, 0f, 125, newColor, 1.2f);
            slimeDust.velocity *= 0.35f;
            slimeDust.velocity += Projectile.velocity * 0.35f;
            slimeDust.noGravity = Main.rand.NextBool(2);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Color newColor = Color.Purple with { A = 0 };
                Dust slimeDust = Dust.NewDustDirect(Projectile.position - Vector2.One * 8 + Projectile.velocity, Projectile.width + 16, Projectile.height + 16, DustID.t_Slime, 0f, 0f, 125, newColor, 2f);
                slimeDust.velocity *= 4f;
                slimeDust.noGravity = Main.rand.NextBool(2);
            }
        }

        /*public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }*/

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Color trailColor = Color.Purple with { A = 127 };

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                trailColor *= 0.8f;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, texture.Frame(), trailColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
