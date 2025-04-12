using Terraria;
using Terraria.ModLoader;
using Project165.Content.Projectiles.Melee;
using Terraria.ID;
using Terraria.Enums;

namespace Project165.Content.Items.Weapons.Melee;

public class IceChakram : ModItem
{
    public override void SetDefaults()
    {
        Item.Size = new(46);
        Item.damage = 65;
        Item.knockBack = 8f;
        Item.useTime = 22;
        Item.useAnimation = 22;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.DamageType = DamageClass.Melee;
        Item.shoot = ModContent.ProjectileType<IceChakramProj>();
        Item.shootSpeed = 15f;
        Item.value = Item.buyPrice(silver: 40);
        Item.rare = ItemRarityID.Pink;
        Item.ArmorPenetration = 10;
        Item.autoReuse = true;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

}
