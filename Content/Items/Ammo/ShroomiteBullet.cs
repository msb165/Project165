using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Ammo
{
    public class ShroomiteBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.Size = new(14);
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.ShroomiteBullet>();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(copper: 15);
            Item.maxStack = Item.CommonMaxStack;
            Item.knockBack = 4f;
            Item.consumable = true;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(70)
                .AddIngredient(ItemID.MusketBall, 70)
                .AddIngredient(ItemID.ShroomiteBar)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe(70)
                .AddIngredient(ItemID.ChlorophyteBullet, 70)
                .AddIngredient(ItemID.GlowingMushroom, 15)
                .AddTile(TileID.Autohammer)
                .Register();
        }
    }
}
