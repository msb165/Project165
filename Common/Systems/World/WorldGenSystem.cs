using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Project165.Common.Systems.World
{
    internal class WorldGenSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int microBiomesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));

            if (microBiomesIndex != -1)
            {
                tasks.Insert(microBiomesIndex + 1, new PassLegacy("Slime Arena", (progress, config) =>
                {
                    progress.Message = "Slime Arena";
                    SlimeArena slimeArena = GenVars.configuration.CreateBiome<SlimeArena>();

                    Point origin2 = default;
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            int j = 0;
                            while (j++ <= Main.maxTilesX)
                            {
                                origin2.Y = (int)GenVars.rockLayer + WorldGen.genRand.Next(300, 500);
                                if (GenVars.dungeonSide < 0)
                                {
                                    origin2.X = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.75), Main.maxTilesX - 50);
                                }
                                else
                                {
                                    origin2.X = WorldGen.genRand.Next(50, (int)(Main.maxTilesX * 0.12));
                                }
                                if (slimeArena.Place(origin2, GenVars.structures))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }));
            }
        }
    }
}
