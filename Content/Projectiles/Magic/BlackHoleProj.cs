using Microsoft.Xna.Framework;
using Project165.Content.Items.Weapons.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class BlackHoleProj : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<BlackHoleStaff>().Texture;
        public override void SetDefaults()
        {
            Projectile.Size = new(10);
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.ignoreWater = true;
        }

        Player Player => Main.player[Projectile.owner];
        public override void AI()
        {
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, Alpha:150);
                dust.scale = 1.5f;
                dust.position = Projectile.Center - Projectile.velocity / 10f * i;
                dust.noGravity = true;
                dust.velocity = Vector2.Normalize(Utils.RandomVector2(Main.rand, -10, 11)) * Main.rand.Next(3, 9);
            }

            if (Main.myPlayer == Projectile.owner)
            { 
                if (Player.channel && Projectile.ai[0] == 0f)
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, Main.MouseWorld, 0.4f);
                }
                else
                {
                    Projectile.netUpdate = true;
                    Projectile.ai[0] = 1f;
                    if (Projectile.velocity.Length() < 2f)
                    {
                        Projectile.velocity = Projectile.DirectionFrom(Player.Center) * 32f;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item107, Projectile.position);

            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, Alpha: 150, Scale:2f);
                dust.noGravity = true;
                dust.velocity *= 4f;
            }

            /*if (Projectile.owner == Main.myPlayer)
            {
                int projAmount = Main.rand.Next(3, 9);
                for (int i = 0; i < projAmount; i++)
                {
                    Vector2 newVelocity = Vector2.Normalize(Utils.RandomVector2(Main.rand, -100, 101));
                    newVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, newVelocity, 511 + Main.rand.Next(3), Projectile.damage, 1f, Projectile.owner);
                }
            }*/
        }
    }
}
