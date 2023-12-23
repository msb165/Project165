﻿using Terraria;
using Terraria.ModLoader;
using Project165.Content.Projectiles.Melee;
using Terraria.ID;

namespace Project165.Content.Items.Weapons.Melee
{
    public class IceChakram : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = 65;
            Item.knockBack = 8f;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<IceChakramProj>();
            Item.shootSpeed = 14f;
            Item.value = Item.buyPrice(0, 0, 40, 0);
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.ResearchUnlockCount = 1;
        }
    }
}
