using Terraria.ID;
using Terraria.ModLoader;
using Project165.Content.Projectiles.Melee;

namespace Project165.Content.Items.Weapons.Melee;

public class ShadowBlade : ModItem
{
    public override void SetDefaults()
    {
        Item.Size = new(46);
        Item.damage = 140;
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
