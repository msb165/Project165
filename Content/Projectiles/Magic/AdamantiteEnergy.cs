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

namespace Project165.Content.Projectiles.Magic
{
    public class AdamantiteEnergy : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BeeArrow}";
        public override void SetDefaults()
        {
            Projectile.alpha = 255;
            Projectile.Size = new(12);
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor:new Color(255, 64, 64), Scale:1f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
                dust.position = Projectile.Center - Projectile.velocity / 20f * i;
            }

            Lighting.AddLight(Projectile.position, Color.Red.ToVector3());

        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.Resize(72, 72);
            Projectile.Damage();
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), newColor: new Color(255, 64, 64), Scale: 1f);
                dust.noGravity = true;
                dust.velocity = (Vector2.UnitX * 4f).RotatedBy(i * MathHelper.TwoPi / 20f);
            }
        }
    }
}
