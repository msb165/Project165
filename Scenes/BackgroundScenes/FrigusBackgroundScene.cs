using Project165.Content.NPCs.Bosses.Frigus;
using Terraria;
using Terraria.ModLoader;

namespace Project165.Scenes.BackgroundScenes
{
    internal class FrigusBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;
        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<IceBossFly>());

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Project165:Frigus", isActive);
        }
    }
}
