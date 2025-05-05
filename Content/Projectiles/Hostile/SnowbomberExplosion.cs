using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Project165.Content.Projectiles.Hostile
{
    internal class SnowbomberExplosion : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.TungstenShortswordStab}";
        public override void SetDefaults()
        {
            Projectile.Size = new(160);
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Alpha: 100, Scale: 1.5f);
            }
            for (int j = 0; j < 24; j++)
            {
                Dust snow = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, Alpha: 100, Scale: 2.5f);
                snow.noGravity = true;
                snow.velocity *= 3f;
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Alpha: 100, Scale: 2f);
                dust.velocity *= 2f;
                dust.noGravity = true;
            }
            for (int k = 0; k < 2; k++)
            {
                Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, default, Main.rand.Next(61, 64), 1.25f);
                gore.velocity *= 0.3f;
                gore.velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                gore.velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        }
    }
}
