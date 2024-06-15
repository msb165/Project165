using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Summon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Summon
{
    public class MachineGunStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MachineGunProj>();
            Item.shootSpeed = 1f;
            Item.sentry = true;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.mana = 5;
            Item.damage = 17;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 6f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
