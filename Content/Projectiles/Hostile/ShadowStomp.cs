using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Project165.Content.Projectiles.Hostile;

public class ShadowStomp : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Bullet}";
    public override void SetDefaults()
    {
        Projectile.Size = new(30);
        Projectile.aiStyle = -1;
        Projectile.alpha = 255;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 120;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 9f)
        {
            Projectile.Kill();
            return;
        }
        Projectile.velocity = Vector2.Zero;
        Projectile.position = Projectile.Center;
        Projectile.Size = new Vector2(16f) * MathHelper.Lerp(5f, 40f, Utils.GetLerpValue(0f, 9f, Projectile.ai[0]));
        Projectile.Center = Projectile.position;
        Point point = Projectile.TopLeft.ToTileCoordinates();
        Point point2 = Projectile.BottomRight.ToTileCoordinates();
        int maxDistance = Projectile.width / 2;
        if ((int)Projectile.ai[0] % 3 != 0)
        {
            return;
        }
        int num4 = (int)Projectile.ai[0] / 3;
        for (int i = point.X; i <= point2.X; i++)
        {
            for (int j = point.Y; j <= point2.Y; j++)
            {
                if (Vector2.Distance(Projectile.Center, new Vector2(i * 16, j * 16)) > maxDistance)
                {
                    continue;
                }
                Tile tileSafely = Framing.GetTileSafely(i, j);
                if (!tileSafely.HasTile || !Main.tileSolid[tileSafely.TileType] || Main.tileSolidTop[tileSafely.TileType] || Main.tileFrameImportant[tileSafely.TileType])
                {
                    continue;
                }
                Tile tileSafely2 = Framing.GetTileSafely(i, j - 1);
                if (tileSafely2.HasTile && Main.tileSolid[tileSafely2.TileType] && !Main.tileSolidTop[tileSafely2.TileType])
                {
                    continue;
                }
                int dustAmount = WorldGen.KillTile_GetTileDustAmount(fail: true, tileSafely, i, j);
                for (int k = 0; k < dustAmount; k++)
                {
                    Dust obj = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
                    obj.noGravity = true;
                    obj.velocity.Y -= 3f + num4 * 1.5f;
                    obj.velocity.Y *= Main.rand.NextFloat();
                    obj.velocity.Y *= 0.75f;
                    obj.scale += num4 * 0.03f;
                }
                if (num4 >= 2)
                {
                    for (int l = 0; l < dustAmount - 1; l++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, 12, 12, DustID.Corruption, 0f, 0f, 50, Scale: 2f);
                        dust.noGravity = true;
                        dust.velocity.Y *= -Main.rand.NextFloat(1f, 4f);
                        dust.velocity.X *= Main.rand.NextFloatDirection() * 3f;
                        dust.position = new Vector2(i * 16 + Main.rand.Next(16), j * 16 + Main.rand.Next(16));
                        if (!Main.rand.NextBool(3))
                        {
                            dust.velocity *= 0.5f;
                            dust.noGravity = true;
                        }
                    }
                }
                if (dustAmount <= 0 || Main.rand.NextBool(3))
                {
                    continue;
                }
            }
        }
    }
}
