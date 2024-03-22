using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Project165.Common.Systems.World
{
    public class SlimeArena : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            Dictionary<ushort, int> tileAmount = new();
            WorldUtils.Gen(new Point(origin.X - 25, origin.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(TileID.Dirt, TileID.Stone).Output(tileAmount));

            // If the amount of dirt and stone tiles are less than 1250, return false
            if (tileAmount[TileID.Dirt] + tileAmount[TileID.Stone] < 1250)
            {
                return false;
            }         
            
            bool flag = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(maxDistance: 1000), new Conditions.IsSolid().AreaOr(1, 40).Not()), out Point result);
            bool tileFlag = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(origin.Y - result.Y), new Conditions.IsTile(TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick)), out var _);
            if (!flag || tileFlag)
            {
                return false;
            }

            result.Y += 50;
            ShapeData circleShape = new();
            ShapeData moundShape = new();
            Point circlePos = new(origin.X, origin.Y + 20);
            Point moundPos = new(origin.X + 40, origin.Y + 60);

            double size = 0.8 + _random.NextDouble() * 0.5;

            WorldUtils.Gen(circlePos, new Shapes.Mound(60, 30), Actions.Chain(new Actions.ClearTile(frameNeighbors: true).Output(circleShape)));
            //WorldUtils.Gen(moundPos, new Shapes.Mound(7, 7), Actions.Chain(new Modifiers.Blotches(1, 1, 0.8), new Actions.SetTile(TileID.SlimeBlock), new Actions.SetFrames(frameNeighbors: true).Output(moundShape)));
            circleShape.Subtract(moundShape, circlePos, moundPos);

            WorldUtils.Gen(circlePos, new ModShapes.InnerOutline(circleShape), Actions.Chain(new Actions.SetTile(TileID.SlimeBlock), new Actions.SetFrames(frameNeighbors: true)));
            WorldUtils.Gen(circlePos, new ModShapes.All(circleShape), Actions.Chain(new Modifiers.RectangleMask(-70, 70, -30, 70), new Modifiers.HasLiquid(), new Actions.Clear()));
            WorldUtils.Gen(circlePos, new ModShapes.All(circleShape), Actions.Chain(new Actions.PlaceWall(WallID.Slime), new Modifiers.OnlyTiles(TileID.SlimeBlock), new Modifiers.Offset(0, 1)));

            //WorldUtils.Gen(new Point(moundPos.X, moundPos.Y - 14), new Shapes.Rectangle(40, 1), Actions.Chain(new Modifiers.IsEmpty(), new Actions.SetTile(TileID.Platforms, false, true)));

            // Arena Entrace
            ShapeData entraceShape = new();
            WorldUtils.Gen(new Point(origin.X, result.Y + 10), new Shapes.Rectangle(4, (origin.Y / 4) - result.Y - 4), Actions.Chain(new Modifiers.SkipTiles(TileID.LihzahrdBrick), new Actions.ClearTile().Output(entraceShape), new Modifiers.Expand(1)));
            WorldUtils.Gen(new Point(origin.X, result.Y + 10), new ModShapes.All(entraceShape), new Actions.SetFrames(frameNeighbors: true));

            WorldUtils.Gen(moundPos, new ModShapes.All(moundShape), Actions.Chain(new Modifiers.Offset(0, -1), new Modifiers.OnlyTiles(TileID.Grass), new Modifiers.Offset(0, -1), new ActionGrass()));
            structures.AddProtectedStructure(new Rectangle(circlePos.X - (int)(20.0 * size), circlePos.Y - 20, (int)(40.0 * size), 40), 10);
            return true;
        }
    }
}
