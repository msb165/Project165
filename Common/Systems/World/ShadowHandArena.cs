using Microsoft.Xna.Framework;
using Project165.Content.Tiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Project165.Common.Systems.World
{
    public class ShadowHandArena : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            bool flag = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(maxDistance: 1000), new Conditions.IsSolid().AreaOr(1, 40).Not()), out Point result);
            if (!flag)
            {
                return false;
            }

            result.Y += 50;
            ShapeData circleShape = new();
            ShapeData moundShape = new();
            //Point circlePos = new(origin.X, origin.Y + 20);
            Point circlePos = new(origin.X, origin.Y);
            //Point altarPos = new(origin.X, origin.Y + 30);
            Point altarPos = new(origin.X, origin.Y + 38);
            Point moundPos = new(origin.X + 40, origin.Y + 60);

            //WorldUtils.Gen(circlePos, new Shapes.Mound(60, 30), Actions.Chain(new Actions.ClearTile(frameNeighbors: true).Output(circleShape)));
            WorldUtils.Gen(circlePos, new Shapes.Rectangle(60, 40), Actions.Chain(new Actions.ClearTile(frameNeighbors: true).Output(circleShape)));
            circleShape.Subtract(moundShape, circlePos, moundPos);

            WorldUtils.Gen(circlePos, new ModShapes.InnerOutline(circleShape), Actions.Chain(new Actions.SetTile((ushort)ModContent.TileType<HardenedGelBlock>()), new Actions.SetFrames(frameNeighbors: true)));
            WorldUtils.Gen(circlePos, new ModShapes.All(circleShape), Actions.Chain(new Modifiers.RectangleMask(-100, 100, -100, 100), new Modifiers.HasLiquid(), new Actions.Clear()));
            WorldUtils.Gen(circlePos, new ModShapes.All(circleShape), Actions.Chain(new Actions.PlaceWall((ushort)ModContent.WallType<ShadowSlimeWall>()), new Modifiers.OnlyTiles((ushort)ModContent.TileType<HardenedGelBlock>()), new Modifiers.Offset(0, 1)));

            // Arena Entrance
            ShapeData entranceShape = new();
            //WorldUtils.Gen(new Point(origin.X + 40, origin.Y + 28), new Shapes.Rectangle(4, 4), Actions.Chain(new Modifiers.SkipTiles(TileID.LihzahrdBrick), new Actions.ClearTile().Output(entraceShape), new Modifiers.Expand(1)));
            WorldUtils.Gen(new Point(origin.X + 40, origin.Y + 39), new Shapes.Rectangle(4, 1), Actions.Chain(new Modifiers.SkipTiles(TileID.LihzahrdBrick), new Actions.SetTile(TileID.Chlorophyte).Output(entranceShape), new Modifiers.Expand(1)));
            WorldUtils.Gen(new Point(origin.X, origin.Y), new ModShapes.All(entranceShape), new Actions.SetFrames(frameNeighbors: true));

            for (int i = 0; i < 5; i++)
            {
                WorldGen.PlaceTile(altarPos.X + 48 - (i * 8), origin.Y + 20, TileID.Torches, false, true, -1, 12);
            }

            WorldGen.PlaceTile(altarPos.X + 30, altarPos.Y, ModContent.TileType<ShadowAltar>());

            structures.AddProtectedStructure(new Rectangle(circlePos.X - 20, circlePos.Y - 20, 40, 40), 10);
            return true;
        }
    }
}
