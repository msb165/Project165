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
            Projectile.Size = new(192);
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 70;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
                Projectile.localAI[0] += 1f;
            }
            for (int i = 0; i < 25; i++)
            {
                Vector2 newVel = Vector2.Normalize(Utils.RandomVector2(Main.rand, -10f, 11f)) * Main.rand.Next(3, 9);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor:Color.Orange, Scale:0.75f);
                dust.noGravity = true;
                dust.position = Projectile.Center + Utils.RandomVector2(Main.rand, -20f, 21f);
                dust.velocity = newVel;
            }
        }
    }
}
