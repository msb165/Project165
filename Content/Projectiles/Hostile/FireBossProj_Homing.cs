using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using Terraria.Audio;

namespace Project165.Content.Projectiles.Hostile
{
    internal class FireBossProjHoming : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BeeArrow}";

        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 spinPoint = Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 16f) * new Vector2(1f, 4f);
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.OrangeTorch, Scale: 3f);
                    dust.position = Projectile.Center - spinPoint;
                    dust.velocity = Vector2.Normalize(spinPoint);
                    dust.noGravity = true;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                //Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, Scale:1.75f);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor:Color.Orange, Scale:0.65f);
                //Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, newColor:Color.Orange);
                dust.position = Projectile.Center - Projectile.velocity / 10f * i;
                dust.velocity *= 0.75f;
                dust.noGravity = true;
            }

            /*float maxDistance = 2000f;
            Player closestPlayer = null;
            for (int j = 0; j < Main.maxPlayers; j++)
            {
                Player target = Main.player[j];
                float distance = Vector2.Distance(Projectile.Center, target.Center);
                if (!target.dead && target.active && distance < maxDistance)
                {
                    maxDistance = distance;
                    closestPlayer = target;
                }
            }

            if (closestPlayer != null)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (closestPlayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 7f, 0.3f);
            }*/
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.Orange);
                dust.velocity *= 4f;
                dust.noGravity = true;
            }
        }
    }
}
