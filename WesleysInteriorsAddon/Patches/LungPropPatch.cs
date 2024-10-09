using HarmonyLib;

namespace com.github.zehsteam.WesleysInteriorsAddon.Patches;

[HarmonyPatch(typeof(LungProp))]
internal static class LungPropPatch
{
    [HarmonyPatch(nameof(LungProp.EquipItem))]
    [HarmonyPrefix]
    private static void EquipItemPatch(ref LungProp __instance)
    {
        if (!__instance.isLungDocked) return;

        Plugin.Instance.OnApparatusRemoved();
    }
}
