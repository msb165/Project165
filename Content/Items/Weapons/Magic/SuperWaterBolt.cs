using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Magic;

namespace Project165.Content.Items.Weapons.Magic;

public class SuperWaterBolt : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 38;
        Item.damage = 60;
        Item.mana = 16;
        Item.DamageType = DamageClass.Magic;
        Item.rare = ItemRarityID.Pink;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.UseSound = SoundID.Item43;
        Item.shoot = ModContent.ProjectileType<SuperWaterBoltProj>();
        Item.value = Item.sellPrice(silver: 25);
        Item.shootSpeed = 12f;
        Item.autoReuse = true;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Vector2 pointPosition = player.RotatedRelativePoint(player.MountedCenter);
        Vector2 newPosition = pointPosition + new Vector2(Main.rand.Next(0, 101) * -player.direction, Main.rand.Next(-100, player.height / 2));

        // This ensures the projectiles will not spawn inside a tile.
        for (int i = 0; i < 50; i++)
        {
            newPosition = pointPosition + new Vector2(Main.rand.Next(0, 101) * -player.direction, Main.rand.Next(-100, player.height / 2));
            if (Collision.CanHit(pointPosition, 0, 0, newPosition + (newPosition - pointPosition).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
            {
                break;
            }
        }
        Vector2 newVelocity = Vector2.Normalize(Main.MouseWorld - newPosition) * Item.shootSpeed;
        Projectile.NewProjectileDirect(source, newPosition, newVelocity, ModContent.ProjectileType<SuperWaterBoltProj>(), damage, knockback, player.whoAmI);

        return false;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SpellTome, 1)
            .AddIngredient(ItemID.WaterBolt, 1)
            .AddIngredient(ItemID.WaterCandle, 1)
            .AddIngredient(ItemID.BottledWater, 5)
            .AddIngredient(ItemID.ChlorophyteBar, 5)
            .AddTile(TileID.Bookcases)
            .Register();
    }
}
