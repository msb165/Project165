using Project165.Content.Items.Weapons.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    internal class BigMagicGunHold : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<BigMagicGun>().Texture;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
