using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using Project165.Content.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Magic
{
    public class Avalanche : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(32);
            Item.DamageType = DamageClass.Magic;
            Item.damage = 70;
            Item.rare = ItemRarityID.Pink;            
            Item.shoot = ModContent.ProjectileType<AvalanceProj>();
            Item.shootSpeed = 20f;
            Item.mana = 12;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.NPCHit11;
            Item.noMelee = true;
            Item.noUseGraphic = true;            
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position += position.SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.PiOver2) * 48f;

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(position, player.width, player.height, DustID.Snow, velocity.X, velocity.Y, 100, default, 1f);
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
