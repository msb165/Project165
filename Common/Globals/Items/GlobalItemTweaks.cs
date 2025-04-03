using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Project165.Utilites;

namespace Project165.Common.Globals.Items
{
    public class GlobalItemTweaks : GlobalItem
    {
        public override bool InstancePerEntity => false;
        public override void SetDefaults(Item entity)
        {
            switch (entity.type)
            {
                case ItemID.ChlorophyteSaber:
                    entity.shoot = ProjectileID.None;
                    entity.shootSpeed = 0;
                    entity.scale = 1.25f;
                    break;
                case ItemID.EmeraldStaff:
                    entity.damage = 10;
                    entity.useTime = 8;
                    entity.useAnimation = 24;
                    entity.reuseDelay = 36;
                    break;
                case ItemID.RubyStaff:
                    entity.damage = 8;
                    entity.useTime = 32;
                    entity.useAnimation = 32;
                    break;
                case ItemID.SuspiciousLookingEye:
                case ItemID.WormFood:
                case ItemID.BloodySpine:
                case ItemID.MechanicalEye:
                case ItemID.MechanicalSkull:
                case ItemID.MechanicalWorm:
                //case ItemID.LihzahrdPowerCell:
                case ItemID.CelestialSigil:
                    entity.consumable = false;
                    break;
            }
        }

        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            if (Item.staff[item.type])
            {
                Project165Utils.SmoothHoldStyle(player);
            }
            float intensity = 0f;
            switch (item.type)
            {
                case ItemID.TacticalShotgun:
                case ItemID.Boomstick:
                    goto recoil;
                case ItemID.Minishark:
                case ItemID.Megashark:
                case ItemID.RedRyder:
                    intensity = -0.7f;
                recoil:
                    Project165Utils.RecoilEffect(player, intensity);
                    break;
            }

        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (item.type == ItemID.ChlorophyteSaber)
            {
                Vector2 newPos = Main.rand.NextVector2CircularEdge(200f, 200f).SafeNormalize(Vector2.Zero) * 6f;
                Projectile.NewProjectile(target.GetSource_OnHit(target), target.Center - newPos * 15, newPos * Main.rand.NextFloat(0.5f, 1.2f), ProjectileID.SporeCloud, item.damage, 0f, player.whoAmI);
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (item.type)
            {
                case ItemID.FrostStaff:
                    Projectile.NewProjectile(source, position + velocity * 4.5f, velocity.RotatedByRandom(0.125f), type, damage, knockback, player.whoAmI);
                    return false;
                case ItemID.SpectreStaff:
                case ItemID.EmeraldStaff:
                case ItemID.SapphireStaff:
                    Projectile.NewProjectile(source, position + velocity * 8f, velocity.RotatedByRandom(0.1f), type, damage, knockback, player.whoAmI);
                    return false;
                case ItemID.DiamondStaff:
                case ItemID.AmberStaff:
                    Projectile.NewProjectile(source, position + velocity * 6f, velocity.RotatedByRandom(0.1f), type, damage, knockback, player.whoAmI);
                    return false;
                case ItemID.AmethystStaff:
                case ItemID.TopazStaff:
                    Projectile.NewProjectile(source, position + velocity * 10f, velocity.RotatedByRandom(0.1f), type, damage, knockback, player.whoAmI);
                    return false;
                case ItemID.RubyStaff:
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(source, position + velocity * 7f, velocity.RotatedByRandom(0.3f), type, damage, knockback, player.whoAmI);
                    }
                    return false;
                case ItemID.InfluxWaver:
                    Projectile.NewProjectile(source, position + velocity * 4f, velocity.RotatedByRandom(0.07f), type, damage, knockback, player.whoAmI);
                    return false;
            }
            return true;
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (item.type == ItemID.ChlorophytePartisan)
            {
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15f));
            }
            if (item.type == ItemID.TacticalShotgun)
            {
                position = position - (Vector2.UnitY * 12f).RotatedBy(new Vector2(velocity.X * player.direction, velocity.Y).ToRotation()) + velocity * 4f;
            }
        }

        public override void AddRecipes()
        {
            Recipe zenithRecipe = Recipe.Create(ItemID.Zenith);
            zenithRecipe.AddIngredient(ItemID.TerraBlade);
            zenithRecipe.AddIngredient(ItemID.Meowmere);
            zenithRecipe.AddIngredient(ItemID.StarWrath);
            zenithRecipe.AddIngredient(ItemID.InfluxWaver);
            zenithRecipe.AddIngredient(ItemID.TheHorsemansBlade);
            zenithRecipe.AddIngredient(ItemID.Seedler);
            zenithRecipe.AddIngredient(ItemID.Starfury);
            zenithRecipe.AddIngredient(ItemID.BeeKeeper);
            zenithRecipe.AddIngredient(ItemID.Terragrim);
            zenithRecipe.AddIngredient(ItemID.CopperShortsword);
        }
    }
}
