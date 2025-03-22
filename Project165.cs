using Microsoft.Xna.Framework;
using Project165.Common.Systems;
using Project165.Content.NPCs.Bosses.Frigus;
using Project165.Skies;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Project165
{
    public class Project165 : Mod
	{
        public override void Load()
        {
            if (!Main.dedServ)
            {
                Filters.Scene["Project165:Frigus"] = new Filter(new FrigusScreenShader("FilterBlizzardForeground").UseColor(1f, 1f, 1f)
                    .UseSecondaryColor(0.7f, 0.7f, 1f)
                    .UseImage("Images/Misc/noise")
                    .UseIntensity(0.4f)
                    .UseImageScale(new Vector2(3f, 0.75f)), EffectPriority.High);
                SkyManager.Instance["Project165:Frigus"] = new FrigusSky();
            }
        }
    }
}