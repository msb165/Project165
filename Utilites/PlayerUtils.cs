
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Project165.Utilites
{
    public partial class Project165Utils
    {
        public static void SmoothHoldStyle(Player player)
        {
            Vector2 direction = Vector2.Normalize(Main.MouseWorld - player.Center);
            player.itemRotation = direction.ToRotation() * player.gravDir;
            if (player.direction == -1)
            {
                player.itemRotation += MathHelper.Pi;
            }
            float rotation = player.itemRotation - MathHelper.PiOver2 * player.gravDir;
            if (player.direction == -1)
            {
                rotation += MathHelper.Pi;
            }
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
            player.ChangeDir(MathF.Sign(direction.X));
        }
    }
}
