using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal static class StartOfRoundPatch
{
    [HarmonyPatch(nameof(StartOfRound.Awake))]
    [HarmonyPostfix]
    private static void AwakePatch()
    {
        SpawnNetworkHandler();
    }

    private static void SpawnNetworkHandler()
    {
        if (!NetworkUtils.IsServer) return;

        var networkHandlerHost = Object.Instantiate(Content.NetworkHandlerPrefab, Vector3.zero, Quaternion.identity);
        networkHandlerHost.GetComponent<NetworkObject>().Spawn();
    }

    [HarmonyPatch(nameof(StartOfRound.OnLocalDisconnect))]
    [HarmonyPrefix]
    private static void OnLocalDisconnectPatch()
    {
        Plugin.Instance.OnLocalDisconnect();
    }

    [HarmonyPatch(nameof(StartOfRound.ShipHasLeft))]
    [HarmonyPostfix]
    private static void ShipHasLeftPatch()
    {
        Plugin.Instance.OnShipHasLeft();
    }
}
