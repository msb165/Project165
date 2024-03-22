using Project165.Content.Projectiles.Melee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Melee
{
    public class ShadowBlade : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(46);
            Item.damage = 110;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<ShadowBladeProj>();
            Item.shootSpeed = 10f;
            Item.knockBack = 8f;
            Item.rare = ItemRarityID.Yellow;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
    }
}
