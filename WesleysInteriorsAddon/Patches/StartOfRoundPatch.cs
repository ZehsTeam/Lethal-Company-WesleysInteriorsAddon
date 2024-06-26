﻿using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRoundPatch
{
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    static void AwakePatch()
    {
        SpawnNetworkHandler();
    }

    private static void SpawnNetworkHandler()
    {
        if (!Plugin.IsHostOrServer) return;

        var networkHandlerHost = Object.Instantiate(Content.networkHandlerPrefab, Vector3.zero, Quaternion.identity);
        networkHandlerHost.GetComponent<NetworkObject>().Spawn();
    }

    [HarmonyPatch("OnLocalDisconnect")]
    [HarmonyPrefix]
    static void OnLocalDisconnectPatch()
    {
        Plugin.Instance.OnLocalDisconnect();
    }

    [HarmonyPatch("ShipHasLeft")]
    [HarmonyPostfix]
    static void ShipHasLeftPatch()
    {
        Plugin.Instance.OnShipHasLeft();
    }
}
