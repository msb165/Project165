using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Common.Globals.NPCs;

public class GlobalNPCBuff : GlobalNPC
{
    public bool moonFire, slowDown;
    public override bool InstancePerEntity => true;

    public override void ResetEffects(NPC npc)
    {
        moonFire = false;
        slowDown = false;
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (moonFire)
        {
            drawColor = Color.Cyan with { A = 200 };
            DrawMoonFire(npc);
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (moonFire)
        {
            ApplyMoonFire(npc, ref damage);
        }
    }

    public override void PostAI(NPC npc)
    {
        if (slowDown)
        {
            npc.velocity = Vector2.Clamp(npc.velocity, -Vector2.One * 3f, Vector2.One * 3f);
        }
    }

    #region Luminite Fire
    public void ApplyMoonFire(NPC npc, ref int damage)
    {
        if (npc.lifeRegen > 0)
        {
            npc.lifeRegen = 0;
        }
        npc.lifeRegen -= 100;
        if (damage < 15)
        {
            damage = 15;
        }
    }

    public void DrawMoonFire(NPC npc)
    {
        if (Main.rand.NextBool(10))
        {
            Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Vortex, Alpha: 100);
            dust.noGravity = true;
            dust.fadeIn = 1.5f;
        }
    }
    #endregion
}
