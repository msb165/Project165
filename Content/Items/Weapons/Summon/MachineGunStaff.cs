using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Summon;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Summon
{
    public class MachineGunStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.shoot = ModContent.ProjectileType<MachineGunProj>();
            Item.shootSpeed = 1f;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.sentry = true;
            Item.mana = 5;
            Item.damage = 17;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile machineGun = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            machineGun.originalDamage = Item.damage;

            player.UpdateMaxTurrets();
            return false;
        }
    }
}
