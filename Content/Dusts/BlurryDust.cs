using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Dusts;

public class BlurryDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.frame = new Rectangle(0, 0, 8, 8);
        dust.noLightEmittence = false;
        dust.alpha = 0;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale *= 0.9f;
        if (dust.scale < 0.01f)
        {
            dust.active = false;
        }
        return false;
    }
}
