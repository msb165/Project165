using Project165.Content.Items.Materials;
using Project165.Content.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Magic
{
    public class Wormhole : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.Size = new(40);
            Item.damage = 53;
            Item.mana = 17;
            Item.shoot = ModContent.ProjectileType<WormholeProj>();
            Item.shootSpeed = 12f;
            Item.knockBack = 4f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.UseSound = SoundID.Item82;
            Item.rare = ItemRarityID.Yellow;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ShadowEssence>(), 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
