
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

        public static void RecoilEffect(Player player, float intensity = 0)
        {
            float animProgress = 1f - player.itemAnimation / (float)player.itemAnimationMax;
            float baseIntensity = 0.85f + intensity;
            //player.itemRotation = (player.itemAnimation / (float)player.itemAnimationMax - 0.5f) * -player.direction * 3.5f - player.direction * 0.3f;
            float recoil = MathHelper.Clamp(Utils.Remap(animProgress, 0f, 0.01f, 0f, 0.75f) * Utils.Remap(animProgress, 0.375f, 0.75f, 0.75f, 0f), 0f, 1f) * player.direction * baseIntensity;
            Vector2 direction = Vector2.Normalize(Main.MouseWorld - player.Center);
            player.itemRotation = direction.ToRotation() * player.gravDir - recoil;
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
