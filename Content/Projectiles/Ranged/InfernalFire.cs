using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Ranged
{
    public class InfernalFire : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.FallingStar}";

        public override void SetDefaults()
        {
            Projectile.Size = new(10);
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 40;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 17;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
                Projectile.localAI[0] += 1f;
            }
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.Orange, Scale: 0.75f);
                dust.noGravity = true;
                dust.position = Projectile.Center - Projectile.velocity / 5f * i;
                dust.velocity *= 0f;

                float timer = MathF.Cos((float)Main.timeForVisualEffects * MathHelper.TwoPi / 30f);
                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.OrangeRed, Scale: 1f);
                dust2.noGravity = true;
                dust2.position = Projectile.Center + Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * i) * (8f + 2f * timer) - Projectile.velocity / 10f * i;
                dust2.velocity *= 0f;

                Dust dust3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.OrangeRed, Scale: 1f);
                dust3.noGravity = true;
                dust3.position = Projectile.Center - Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * i) * (8f + 2f * timer) - Projectile.velocity / 10f * i;
                dust3.velocity *= 0f;
            }
        }
    }
}
