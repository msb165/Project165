using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Project165.Content.Items.Weapons.Magic
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

        public override void AI()
        {

            int playerIndex = (int)Projectile.ai[0];
            float moveSpeed = 4f;
            Vector2 playerProjDistance = Main.player[playerIndex].Center - Projectile.Center;
            float playerProjDist = playerProjDistance.Length();
            
            if (playerProjDist < 50f && Projectile.Hitbox.Intersects(Main.player[playerIndex].Hitbox))
            {
                if (Projectile.owner == Main.myPlayer && !Main.player[Main.myPlayer].moonLeech)
                {
                    int manaAmount = (int)Projectile.ai[1];
                    Main.player[playerIndex].ManaEffect(manaAmount);
                    Player playerOwner = Main.player[playerIndex];
                    playerOwner.statMana += manaAmount;
                    if (Main.player[playerIndex].statMana > Main.player[playerIndex].statManaMax2)
                    {
                        Main.player[playerIndex].statMana = Main.player[playerIndex].statManaMax2;
                    }
                    NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, playerIndex, manaAmount);
                }
                Projectile.Kill();
            }
            playerProjDist = moveSpeed / playerProjDist;
            playerProjDistance *= playerProjDist;
            Projectile.velocity.X = (Projectile.velocity.X * 15f + playerProjDistance.X) / 16f;
            Projectile.velocity.Y = (Projectile.velocity.Y * 15f + playerProjDistance.Y) / 16f;

            for (int i = 0; i < 5; i++)
            {
                float speedX = Projectile.velocity.X * 0.2f * i;
                float speedY = -Projectile.velocity.Y * 0.2f * i;
                Dust manaDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, 0f, 0f, 100, Color.Blue, 1.5f);
                manaDust.noGravity = true;

                Dust secondManaDust = manaDust;
                secondManaDust.velocity *= 0f;
                manaDust.position.X -= speedX;
                manaDust.position.Y -= speedY;
            }
        }
    }
}
