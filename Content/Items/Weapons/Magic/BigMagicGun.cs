using Project165.Content.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Magic
{
    internal class BigMagicGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 45;            
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.shoot = ModContent.ProjectileType<BigMagicGunHold>();
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 7);
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }
    }
}
