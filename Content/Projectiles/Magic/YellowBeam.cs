using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Project165.Content.Items.Weapons.Magic;
using Terraria.Audio;


namespace Project165.Content.Projectiles.Magic
{
    public class YellowBeam : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<StellarRod>().Texture;
        public override void SetDefaults()
        {
            Projectile.Size = new(8);
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 5;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            for (int i = 0; i < 7; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SandSpray, newColor:Color.White with { A = 0 }, Scale: 0.8f);
                dust.position = Projectile.Center - Projectile.velocity / 10f * i;
                dust.velocity *= 0.3f;
                dust.noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3());
            Projectile.velocity.Y += 0.2f;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.Damage();
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SandSpray, newColor: Color.White with { A = 0 }, Scale: 0.8f);
                dust.velocity *= 2f;
                dust.noGravity = true;
            }
        } 
    }
}
