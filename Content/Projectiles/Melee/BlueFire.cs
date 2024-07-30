using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee
{
    public class BlueFire : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.AmberBolt}";
        public override void SetDefaults()
        {
            Projectile.Size = new(8);
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
        }

        public override void AI()
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 newVelocity = Vector2.Normalize(Utils.RandomVector2(Main.rand, -10, 11)) * Main.rand.Next(3, 10);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
                dust.noGravity = true;
                dust.position = Projectile.Center;
                dust.velocity = newVelocity;
            }
        }
    }
}
