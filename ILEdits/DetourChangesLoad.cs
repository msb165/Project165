using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Project165.ILEdits
{
    public partial class DetourChanges : ModSystem
    {
        public override void OnModLoad()
        {
            On_ItemDropDatabase.RegisterPresent += On_ItemDropDatabase_RegisterPresent;
        }
    }
}
