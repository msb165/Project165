using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee
{
    public class IceChakramProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
        }

        private Player Owner => Main.player[Projectile.owner];
        private readonly float returnSpeed = 18f;
        private readonly float acceleration = 1.2f;

        public override void AI()
        {
            Projectile.rotation += 0.25f * Owner.direction;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 30f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {                
                Vector2 playerProjDistance = Owner.Center - Projectile.Center;
                float distance = Vector2.Distance(Owner.Center, Projectile.Center);

                if (distance > 3000f)
                {
                    Projectile.Kill();
                }

                distance = returnSpeed / distance;
                playerProjDistance.X *= distance;
                playerProjDistance.Y *= distance;
                if (Projectile.velocity.X < playerProjDistance.X)
                {
                    Projectile.velocity.X += acceleration;
                    if (Projectile.velocity.X < 0f && playerProjDistance.X > 0f)
                    {
                        Projectile.velocity.X += acceleration;
                    }
                }
                else if (Projectile.velocity.X > playerProjDistance.X)
                {
                    Projectile.velocity.X -= acceleration;
                    if (Projectile.velocity.X > 0f && playerProjDistance.X < 0f)
                    {
                        Projectile.velocity.X -= acceleration;
                    }
                }
                if (Projectile.velocity.Y < playerProjDistance.Y)
                {
                    Projectile.velocity.Y += acceleration;
                    if (Projectile.velocity.Y < 0f && playerProjDistance.Y > 0f)
                    {
                        Projectile.velocity.Y += acceleration;
                    }
                }
                else if (Projectile.velocity.Y > playerProjDistance.Y)
                {
                    Projectile.velocity.Y -= acceleration;
                    if (Projectile.velocity.Y > 0f && playerProjDistance.Y < 0f)
                    {
                        Projectile.velocity.Y -= acceleration;
                    }
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                    {
                        Projectile.Kill();
                    }
                }
            }

            GenerateDust();
        }

        private void GenerateDust()
        {
            Vector2 dustPosition = Projectile.Center; //+ Vector2.Normalize(Projectile.velocity) * 10f;
            Vector2 dustVelocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 negativeDustVelocity = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);

            Dust iceDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch);
            iceDust.position = dustPosition;
            iceDust.velocity = dustVelocity * 0.33f + Projectile.velocity / 4f;
            iceDust.position += dustVelocity;
            iceDust.fadeIn = 0.5f;
            iceDust.noGravity = true;
            Dust iceDust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch);
            iceDust2.position = dustPosition;
            iceDust2.velocity = negativeDustVelocity * 0.33f + Projectile.velocity / 4f;
            iceDust2.position += negativeDustVelocity;
            iceDust2.fadeIn = 0.5f;
            iceDust2.noGravity = true;
        }
    }
}
