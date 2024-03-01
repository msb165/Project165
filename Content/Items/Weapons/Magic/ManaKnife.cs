using Project165.Content.Projectiles.Magic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Magic
{
    public class ManaKnife : ModItem
    {
        public override void SetDefaults()
        {
            Item.mana = 6;
            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<ManaKnifeProj>();
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.UseSound = SoundID.Item1 with { Pitch = 0.8f };
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
