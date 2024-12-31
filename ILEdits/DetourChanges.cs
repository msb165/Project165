using Humanizer;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Project165.ILEdit
{
    public class DetourChanges : ModSystem
    {
        public override void OnModLoad()
        {
            On_ItemDropDatabase.RegisterPresent += On_ItemDropDatabase_RegisterPresent;
        }

        // Original code by Snek
        private void On_ItemDropDatabase_RegisterPresent(On_ItemDropDatabase.orig_RegisterPresent orig, ItemDropDatabase self)
        {
            // Move the Snow Globe to Pre-Hardmode
            IItemDropRule snowGlobeRule = ItemDropRule.Common(ItemID.SnowGlobe, chanceDenominator: 15);

            IItemDropRule redRyderRule = ItemDropRule.NotScalingWithLuck(ItemID.RedRyder, chanceDenominator: 150);
            redRyderRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ItemID.MusketBall, minimumDropped: 30, maximumDropped: 60));

            IItemDropRule mrsClauseRule = ItemDropRule.NotScalingWithLuck(ItemID.MrsClauseHat);
            mrsClauseRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ItemID.MrsClauseShirt));
            mrsClauseRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ItemID.MrsClauseHeels));

            IItemDropRule parkaRule = ItemDropRule.NotScalingWithLuck(ItemID.ParkaHood);
            parkaRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ItemID.ParkaCoat));
            parkaRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ItemID.ParkaPants));

            IItemDropRule treeRule = ItemDropRule.NotScalingWithLuck(ItemID.TreeMask);
            treeRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ItemID.TreeShirt));
            treeRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ItemID.TreeTrunks));

            IItemDropRule[] vanityRules =
            [
                mrsClauseRule,
                parkaRule,
                treeRule,
                ItemDropRule.NotScalingWithLuck(ItemID.SnowHat),
                ItemDropRule.NotScalingWithLuck(ItemID.UglySweater)
            ];

            IItemDropRule vanityRule = new OneFromRulesRule(chanceDenominator: 15, vanityRules);

            IItemDropRule foodRule = ItemDropRule.OneFromOptionsNotScalingWithLuck(chanceDenominator: 7, ItemID.ChristmasPudding, ItemID.SugarCookie, ItemID.GingerbreadCookie);

            IItemDropRule blockRule = new OneFromRulesRule(1, ItemDropRule.Common(ItemID.PineTreeBlock, 1, 20, 49), ItemDropRule.Common(ItemID.CandyCaneBlock, 1, 20, 49), ItemDropRule.Common(ItemID.GreenCandyCaneBlock, 1, 20, 49));

            IItemDropRule[] rules =
            [
                snowGlobeRule,
                ItemDropRule.NotScalingWithLuck(ItemID.Coal, chanceDenominator: 30),
                ItemDropRule.NotScalingWithLuck(ItemID.DogWhistle, chanceDenominator: 400),
                redRyderRule,
                ItemDropRule.NotScalingWithLuck(ItemID.CandyCaneSword, chanceDenominator: 150),
                ItemDropRule.NotScalingWithLuck(ItemID.CnadyCanePickaxe, chanceDenominator: 150),
                ItemDropRule.NotScalingWithLuck(ItemID.CandyCaneHook, chanceDenominator: 150),
                ItemDropRule.NotScalingWithLuck(ItemID.FruitcakeChakram, chanceDenominator: 150),
                ItemDropRule.NotScalingWithLuck(ItemID.HandWarmer, chanceDenominator: 150),
                ItemDropRule.NotScalingWithLuck(ItemID.Toolbox, chanceDenominator: 300),
                ItemDropRule.NotScalingWithLuck(ItemID.ReindeerAntlers, chanceDenominator: 40),
                ItemDropRule.NotScalingWithLuck(ItemID.Holly, chanceDenominator: 10),
                vanityRule,
                foodRule,
                ItemDropRule.NotScalingWithLuck(ItemID.Eggnog, chanceDenominator: 8, maximumDropped: 3),
                ItemDropRule.NotScalingWithLuck(ItemID.StarAnise, chanceDenominator: 9, minimumDropped: 20, maximumDropped: 40),
                blockRule
            ];
            
            self.RegisterToItem(ItemID.Present, new SequentialRulesNotScalingWithLuckRule(chanceDenominator: 1, rules));
            return;
        }
    }
}
