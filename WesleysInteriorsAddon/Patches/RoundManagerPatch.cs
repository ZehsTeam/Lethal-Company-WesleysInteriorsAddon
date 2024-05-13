using HarmonyLib;

namespace com.github.zehsteam.WesleysInteriorsAddon.Patches;

[HarmonyPatch(typeof(RoundManager))]
internal class RoundManagerPatch
{
    [HarmonyPatch("FinishGeneratingLevel")]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    static void FinishGeneratingLevelPatch()
    {
        Plugin.Instance.OnFinishGeneratingLevel();
    }
}
