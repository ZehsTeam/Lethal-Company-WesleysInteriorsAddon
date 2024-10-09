using HarmonyLib;

namespace com.github.zehsteam.WesleysInteriorsAddon.Patches;

[HarmonyPatch(typeof(RoundManager))]
internal static class RoundManagerPatch
{
    [HarmonyPatch(nameof(RoundManager.FinishGeneratingLevel))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static void FinishGeneratingLevelPatch()
    {
        Plugin.Instance.OnFinishGeneratingLevel();
    }
}
