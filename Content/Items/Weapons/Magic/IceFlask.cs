using Project165.Content.Projectiles.Magic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Magic
{
    public class IceFlask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 32;
            Item.damage = 16;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.shoot = ModContent.ProjectileType<IceFlaskProj>();
            Item.shootSpeed = 9f;
            Item.UseSound = SoundID.Item106;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.rare = ItemRarityID.Green;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }
    }
}
