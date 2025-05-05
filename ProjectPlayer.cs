using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Typeless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165
{
    public class ProjectPlayer : ModPlayer
    {
        int iceCloakCooldown;
        public Item iceCloakItem;
        public bool hasIceCloak;

        public override void PostUpdate()
        {
            if (iceCloakCooldown > 0)
            {
                iceCloakCooldown--;
                if (iceCloakCooldown == 0)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana);
                }
            }
        }

        public override void ResetEffects()
        {
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (hasIceCloak && iceCloakItem != null && !iceCloakItem.IsAir && iceCloakCooldown == 0 && Player.whoAmI == Main.myPlayer)
            {
                iceCloakCooldown = 60;
                for (int i = 0; i < 3; i++)
                {
                    int damage = 80;
                    if (Main.masterMode)
                    {
                        damage *= 2;
                    }
                    else if (Main.expertMode)
                    {
                        damage = (int)(damage * 1.5f);
                    }
                    Vector2 targetPos = new(Player.position.X + Main.rand.Next(-400, 401), Player.position.Y - Main.rand.Next(500, 801));
                    Vector2 targetVel = Vector2.Normalize(Player.Center + Vector2.UnitX * Main.rand.Next(-100, 101) - targetPos) * 20f;
                    Projectile.NewProjectileDirect(Player.GetSource_Accessory(iceCloakItem), targetPos, targetVel, ModContent.ProjectileType<IceCloakProj>(), damage, 4f, Player.whoAmI);
                }
            }
        }
    }
}
