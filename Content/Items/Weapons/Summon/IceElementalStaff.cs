using Microsoft.Xna.Framework;
using Project165.Buffs;
using Project165.Content.Projectiles.Summon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Project165.Content.Items.Weapons.Summon
{
    internal class IceElementalStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.damage = 25;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<IceElementalProj>();
            Item.mana = 10;
            Item.knockBack = 4f;
            Item.shootSpeed = 5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Yellow;
            Item.buffType = ModContent.BuffType<IceElementalBuff>();
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            Projectile proj = Projectile.NewProjectileDirect(source, position, Vector2.Zero, type, Item.damage, knockback, player.whoAmI, ai0: 0.05f * player.ownedProjectileCounts[type]);
            proj.originalDamage = Item.damage;
            return false;
        }
    }
}
