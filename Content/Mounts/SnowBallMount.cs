using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Buffs;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Mounts
{
    public class SnowBallMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = DustID.Snow;
            MountData.spawnDustNoGravity = true;
            MountData.buff = ModContent.BuffType<SnowBallBuff>();
            MountData.flightTimeMax = 0;
            MountData.fallDamage = 0.1f;
            MountData.runSpeed = 9f;
            MountData.acceleration = 0.18f;
            MountData.jumpHeight = 18;
            MountData.jumpSpeed = 9f;
            MountData.blockExtraJumps = true;
            MountData.totalFrames = 1;
            MountData.playerYOffsets = [8];
            MountData.xOffset = 1;
            MountData.bodyFrame = 3;
            MountData.yOffset = 0;
            MountData.playerHeadOffset = 18;
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 0;
            MountData.standingFrameStart = 0;

            MountData.runningFrameCount = 1;
            MountData.runningFrameDelay = 0;
            MountData.runningFrameStart = 0;

            MountData.flyingFrameCount = 1;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;

            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 0;
            MountData.inAirFrameStart = 0;

            MountData.idleFrameCount = 0;
            MountData.idleFrameDelay = 0;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = false;

            MountData.swimFrameCount = 0;
            MountData.swimFrameDelay = 0;
            MountData.swimFrameStart = 0;

            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.frontTexture.Width();
                MountData.textureHeight = MountData.frontTexture.Height();
            }
        }

        public override void UpdateEffects(Player player)
        {
            //float rotation = MathHelper.Clamp(player.velocity.X * 0.05f, -4f, 4f);
            //player.fullRotation = player.velocity.X * 0.01f;
        }

        float rotationSpeed = 0f;
        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            if (drawType == 2)
            {
                if (drawPlayer.velocity.X != 0f)
                {
                    rotationSpeed += MathHelper.Lerp(0f, 0.2f, drawPlayer.velocity.X * 0.25f);
                }

                playerDrawData.Add(new DrawData(texture, drawPosition + Vector2.UnitY * 4, texture.Frame(), drawColor, rotationSpeed, drawOrigin, drawScale, spriteEffects));
                return false;
            }
            return true;
        }
    }
}
