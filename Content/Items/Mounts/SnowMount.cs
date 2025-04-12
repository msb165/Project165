using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Mounts
{
    public class SnowMount : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item25;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.Size = new(30);
            Item.mountType = ModContent.MountType<Content.Mounts.SnowBallMount>();
            Item.rare = ItemRarityID.Green;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
    }
}
