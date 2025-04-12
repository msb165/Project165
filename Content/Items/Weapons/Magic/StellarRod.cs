using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Magic;

public class StellarRod : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 46;
        Item.damage = 8;
        Item.DamageType = DamageClass.Magic;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item1;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.shoot = ModContent.ProjectileType<StellarRodHoldoutProj>();
        Item.shootSpeed = 1f;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(silver: 45);
    }

    public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
}
