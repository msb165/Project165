using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.NPCs.Bosses.Frigus;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Project165.Skies;

public class FrigusSky : CustomSky
{
    public bool isActive;
    private int iceBossIndex;
    private float intensity;

    private bool UpdateIceIndex()
    {
        int iceBossType = ModContent.NPCType<IceBossFly>();
        if (iceBossIndex >= 0 && Main.npc[iceBossIndex].active && Main.npc[iceBossIndex].type == iceBossType)
        {
            return true;
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
        return iceBossIndex != -1;
    }

    public override void Update(GameTime gameTime)
    {
        if (Main.gamePaused || !Main.hasFocus)
        {
            return;
        }

        if (iceBossIndex == -1)
        {
            UpdateIceIndex();
            if (iceBossIndex == -1)
            {
                isActive = false;
            }
        }

        if (isActive && intensity < 1f)
        {
            intensity += 0.01f;
        }
        else if (!isActive && intensity > 0f)
        {
            intensity -= 0.01f;
        }
    }

    public override float GetCloudAlpha() => 0.4f;

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        if (minDepth < 1f && maxDepth >= 0f)
        {
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), new Color(250, 250, 255, 255) * 0.5f * intensity);
        }
    }

    public override void Activate(Vector2 position, params object[] args) => isActive = true;

    public override void Deactivate(params object[] args) => isActive = false;

    public override void Reset() => isActive = false;

    public override bool IsActive() => isActive || intensity > 0f;
}
