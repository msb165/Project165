using Project165.Content.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Melee;

public class Roqueforti : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.Yoyo[Type] = true;
        ItemID.Sets.GamepadExtraRange[Type] = 15;
        ItemID.Sets.GamepadSmartQuickReach[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 26;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 25;
        Item.useAnimation = 25;            
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.UseSound = SoundID.Item1;
        Item.damage = 50;
        Item.DamageType = DamageClass.MeleeNoSpeed;
        Item.knockBack = 2.5f;
        Item.crit = 8;
        Item.rare = ItemRarityID.Lime;
        Item.shoot = ModContent.ProjectileType<RoquefortiProjectile>();
        Item.shootSpeed = 16f;
        Item.value = Item.buyPrice(gold: 30);
        Item.channel = true;
        Item.autoReuse = true;
    }
}
