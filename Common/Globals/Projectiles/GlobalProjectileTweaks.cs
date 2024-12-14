using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Common.Globals.Projectiles
{
    public class GlobalProjectileTweaks : GlobalProjectile
    {
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.type == ProjectileID.FrostBoltStaff)
            {
                FrostBoltAI(projectile);
                return false;
            }
            return true;
        }

        public void FrostBoltAI(Projectile projectile)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Frost, Alpha: 50, newColor: Color.Cyan, Scale: 1.2f);
                dust.noGravity = true;
                dust.position = projectile.Center - projectile.velocity / 10f * i;
                dust.velocity *= 0.3f;
                Dust dust2 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SpectreStaff, Alpha: 50, newColor: Color.Cyan, Scale: 1.5f);
                dust2.noGravity = true;
                dust2.position = projectile.Center - projectile.velocity / 10f * i;
                dust2.velocity *= 0.6f;
            }
        }
    }
}
