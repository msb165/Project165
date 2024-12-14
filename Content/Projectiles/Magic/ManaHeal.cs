using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Project165.Content.Projectiles.Magic
{
    internal class ManaHeal : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.SpiritHeal}";

        public override void SetDefaults()
        {
            Projectile.Size = new(6);
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 10;
            Projectile.ignoreWater = true;
        }

        Player Owner => Main.player[(int)Projectile.ai[0]];

        public override void AI()
        {
            float moveSpeed = 4f;
            Vector2 playerProjDistance = Owner.Center - Projectile.Center;
            float playerProjDist = playerProjDistance.Length();

            if (playerProjDist < 50f && Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                if (Projectile.owner == Main.myPlayer && !Main.player[Main.myPlayer].moonLeech)
                {
                    int manaAmount = (int)Projectile.ai[1];
                    Owner.ManaEffect(manaAmount);
                    Player playerOwner = Owner;
                    playerOwner.statMana += manaAmount;
                    if (Owner.statMana > Owner.statManaMax2)
                    {
                        Owner.statMana = Owner.statManaMax2;
                    }
                    NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, Owner.whoAmI, manaAmount);
                }
                Projectile.Kill();
            }

            playerProjDist = moveSpeed / playerProjDist;
            playerProjDistance *= playerProjDist;
            Projectile.velocity = (Projectile.velocity * 15f + playerProjDistance) / 16f;

            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = new(Projectile.velocity.X * 0.25f * i, -Projectile.velocity.Y * 0.25f * i);
                Dust manaDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, 0f, 0f, 100, Color.Blue, 1.5f);
                manaDust.noGravity = true;
                Dust secondManaDust = manaDust;
                secondManaDust.velocity *= 0f;
                manaDust.position.X -= speed.X;
                manaDust.position.Y -= speed.Y;
            }
        }
    }
}
