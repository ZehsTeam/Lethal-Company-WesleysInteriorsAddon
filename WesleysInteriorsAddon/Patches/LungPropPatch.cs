using HarmonyLib;

namespace com.github.zehsteam.WesleysInteriorsAddon.Patches;

[HarmonyPatch(typeof(LungProp))]
internal class LungPropPatch
{
    [HarmonyPatch("EquipItem")]
    [HarmonyPrefix]
    static void EquipItemPatch(ref LungProp __instance)
    {
        if (!__instance.isLungDocked) return;

        Toystore.OnApparatusRemoved();
    }
}
