using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Utilites;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Project165.Content.Tiles.Furniture
{
    public class BaseRelic : ModTile
    {
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 4;
        public const int HorizontalFrames = 1;
        public const int VerticalFrames = 1;

        public Asset<Texture2D> RelicTexture;
        public virtual string RelicTextureName => Project165Utils.ContentPath + "Tiles/Furniture/FrigusRelic";
        public override string Texture => $"Terraria/Images/Tiles_{TileID.MasterTrophyBase}";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
            }
        }

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = false;

            TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.styleLineSkipVisualOverride = 0;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Gold, Language.GetText("MapObject.Relic"));
        }

        public override bool CreateDust(int i, int j, ref int type) => false;

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            tileFrameX %= FrameWidth;
            tileFrameY %= FrameHeight * 2;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new(Main.offScreenRange);
            if (Main.drawToScreen)
            {
                offScreen = Vector2.Zero;
            }

            Point p = new(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (!tile.HasTile)
            {
                return;
            }

            Texture2D texture = RelicTexture.Value;
            int frameY = tile.TileFrameX / FrameWidth;
            bool direction = tile.TileFrameY / FrameHeight != 0;
            Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);
            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

            float offset = MathF.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
            float colorOffset = MathF.Sin(Main.GlobalTimeWrappedHourly * MathHelper.Pi) * 0.3f + 0.7f;

            Vector2 vector2 = worldPos + (Vector2.UnitY * -40f) + new Vector2(0f, offset * 4f);
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color lightColor = Lighting.GetColor(p.X, p.Y);
            Color color2 = lightColor with { A = 0 };
            color2 = color2 * 0.1f * colorOffset;

            spriteBatch.Draw(texture, vector2 + offScreen - Main.screenPosition, frame, lightColor, 0f, origin, 1f, effects, 0f);

            for (float m = 0f; m < 1f; m += 1f / 6f)
            {
                spriteBatch.Draw(texture, vector2 + offScreen - Main.screenPosition + (MathHelper.TwoPi * m).ToRotationVector2() * (6f + offset * 2f), frame, color2, 0f, origin, 1f, effects, 0f);
            }
        }
    }
}
