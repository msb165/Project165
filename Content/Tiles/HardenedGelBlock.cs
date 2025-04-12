using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Project165.Content.Tiles;

public class HardenedGelBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileBlockLight[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileSolid[Type] = true;

        LocalizedText name = CreateMapEntryName();
        AddMapEntry(Color.Purple, name);

        DustType = DustID.Crimslime;
        MinPick = 215;
    }

    public override bool CanExplode(int i, int j) => false;
}
