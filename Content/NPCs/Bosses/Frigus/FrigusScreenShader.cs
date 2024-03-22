using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Project165.Content.NPCs.Bosses.Frigus
{
    public class FrigusScreenShader : ScreenShaderData
    {
        public FrigusScreenShader(string passName) : base(passName)
        {
        }

        private Vector2 _texturePosition = Vector2.Zero;
        private int iceBossIndex;
        private float windSpeed = 0.1f;

        private void UpdateIceIndex()
        {
            int iceBossType = ModContent.NPCType<IceBossFly>();
            if (iceBossIndex >= 0 && Main.npc[iceBossIndex].active && Main.npc[iceBossIndex].type == iceBossType)
            {
                return;
            }
            iceBossIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == iceBossType)
                {
                    iceBossIndex = i;
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (iceBossIndex == -1)
            {
                UpdateIceIndex();
                if (iceBossIndex == -1)
                {
                    Filters.Scene["Project165:Frigus"].Deactivate();
                }
            }
            else
            {
                float currentWindSpeed = Main.windSpeedCurrent;
                if (currentWindSpeed >= 0f && currentWindSpeed <= 0.1f)
                {
                    currentWindSpeed = 0.1f;
                }
                else if (currentWindSpeed <= 0f && currentWindSpeed >= -0.1f)
                {
                    currentWindSpeed = -0.1f;
                }
                windSpeed = currentWindSpeed * 0.05f + windSpeed * 0.95f;
                Vector2 vector = new Vector2(0f - windSpeed, -1f) * new Vector2(10f, 2f);
                vector.Normalize();
                vector *= new Vector2(0.8f, 0.6f);
                if (!Main.gamePaused && Main.hasFocus)
                {
                    _texturePosition += vector * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                _texturePosition.X %= 10f;
                _texturePosition.Y %= 10f;
                UseDirection(vector);
                UseTargetPosition(_texturePosition);
            }
        }

        public override void Apply()
        {
            UpdateIceIndex();
            if (iceBossIndex != -1)
            {
                UseTargetPosition(_texturePosition);
            }
            base.Apply();
        }
    }
}
