using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Project165.Content.Projectiles.Melee;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Project165.Content.Items.Weapons.Melee
{
    public class LuminiteShortsword : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(36);
            Item.rare = ItemRarityID.Red;
            Item.damage = 170;
            Item.crit = 10;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.shoot = ModContent.ProjectileType<LuminiteShortProj>();
            Item.shootSpeed = 15f;
            Item.value = Item.sellPrice(gold: 5);
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useAnimation = 18;
            Item.useTime = 6;
            Item.knockBack = 4f;
            Item.ArmorPenetration = 10;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, player.GetAdjustedItemScale(Item));
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 7)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
