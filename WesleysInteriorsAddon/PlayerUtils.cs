using GameNetcodeStuff;
using System.Collections.Generic;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal static class PlayerUtils
{
    public static PlayerControllerB GetLocalPlayerScript()
    {
        if (GameNetworkManager.Instance == null) return null;
        return GameNetworkManager.Instance.localPlayerController;
    }

    public static bool IsLocalPlayer(PlayerControllerB playerScript)
    {
        return playerScript == GetLocalPlayerScript();
    }

    public static PlayerControllerB GetPlayerScriptByClientId(ulong clientId)
    {
        foreach (var playerScript in StartOfRound.Instance.allPlayerScripts)
        {
            if (playerScript.actualClientId == clientId)
            {
                return playerScript;
            }
        }

        return null;
    }

    public static List<PlayerControllerB> GetPlayerScripts()
    {
        List<PlayerControllerB> playerScripts = [];

        foreach (var playerScript in StartOfRound.Instance.allPlayerScripts)
        {
            if (!playerScript.isInHangarShipRoom && !playerScript.isInsideFactory && !playerScript.isInElevator)
            {
                continue;
            }

            playerScripts.Add(playerScript);
        }

        return playerScripts;
    }

    public static PlayerControllerB GetRandomPlayerScript(bool onlyAlivePlayers = true, bool onlyPlayersInsideFactory = true)
    {
        List<PlayerControllerB> playerScripts = [];

        foreach (var playerScript in GetPlayerScripts())
        {
            if (onlyAlivePlayers && playerScript.isPlayerDead) continue;
            if (onlyPlayersInsideFactory && !playerScript.isInsideFactory) continue;

            playerScripts.Add(playerScript);
        }

        if (playerScripts.Count == 0) return null;

        return playerScripts[Random.Range(0, playerScripts.Count)];
    }
}
