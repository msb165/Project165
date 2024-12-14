using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Common.Globals.Items
{
    public class GlobalItemTweaks : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            if (entity.type == ItemID.ChlorophyteSaber)
            {
                entity.shoot = ProjectileID.None;
                entity.shootSpeed = 0;
                entity.scale = 1.25f;
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

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (item.type == ItemID.ChlorophytePartisan)
            {
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15f));
            }
        }
    }
}
