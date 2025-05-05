using Project165.Buffs;
using Project165.Content.Projectiles.Summon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Project165.Content.Items.Weapons.Summon
{
    public class IceGuardianStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.damage = 32;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<IceGuardianProj>();
            Item.mana = 10;
            Item.knockBack = 4f;
            Item.shootSpeed = 5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Yellow;
            Item.buffType = ModContent.BuffType<IceGuardianBuff>();
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spinPoint = Vector2.Zero.RotatedBy(MathHelper.PiOver2);
            player.AddBuff(Item.buffType, 2);
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback, spinPoint, spinPoint);
            return false;
        }
    }
}
