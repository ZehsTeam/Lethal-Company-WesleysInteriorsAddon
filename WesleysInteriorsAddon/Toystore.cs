using com.github.zehsteam.WesleysInteriorsAddon.MonoBehaviours;
using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal static class ToyStore
{
    private static List<GameObject> _nutcrackerObjects = [];
    private static Coroutine _activateNutcrackerStatuesOnServerCoroutine = null;

    public static void Reset()
    {
        _nutcrackerObjects = [];

        if (_activateNutcrackerStatuesOnServerCoroutine != null)
        {
            StartOfRound.Instance.StopCoroutine(_activateNutcrackerStatuesOnServerCoroutine);
        }
    }

    public static void FindNutcrackerStatues()
    {
        if (!InteriorHelper.IsToyStoreInterior()) return;

        _nutcrackerObjects = GetNutcrackerStatues();

        Plugin.logger.LogInfo($"Found {_nutcrackerObjects.Count} Nutcracker statues.");
    }

    public static void OnApparatusRemoved()
    {
        if (!NetworkUtils.IsServer) return;
        if (!InteriorHelper.IsToyStoreInterior()) return;

        if (!Plugin.ConfigManager.ToyStoreNutcracker_Enabled.Value)
        {
            return;
        }

        _activateNutcrackerStatuesOnServerCoroutine = StartOfRound.Instance.StartCoroutine(ActivateNutcrackerStatuesOnServerCoroutine());
    }

    private static IEnumerator ActivateNutcrackerStatuesOnServerCoroutine()
    {
        Plugin.logger.LogInfo("Started Nutcracker spawn coroutine");

        if (!NetworkUtils.IsServer)
        {
            Plugin.logger.LogError("Ended Nutcracker spawn coroutine. You are not the host!");
            yield break;
        }

        if (!Plugin.ConfigManager.ToyStoreNutcracker_Enabled.Value)
        {
            Plugin.logger.LogWarning("Ended Nutcracker spawn coroutine. Nutcracker spawning is disabled in the config settings.");
            yield break;
        }

        if (_nutcrackerObjects == null || _nutcrackerObjects.Count == 0)
        {
            Plugin.logger.LogWarning("Ended Nutcracker spawn coroutine. No Nutcracker statues were found.");
            yield break;
        }

        float minInitialSpawnDelay = Plugin.ConfigManager.ToyStoreNutcracker_MinInitialSpawnDelay.Value;
        float maxInitialSpawnDelay = Plugin.ConfigManager.ToyStoreNutcracker_MaxInitialSpawnDelay.Value;
        float initialSpawnDelay = Mathf.Max(Random.Range(minInitialSpawnDelay, maxInitialSpawnDelay), 0f);

        Plugin.Instance.LogInfoExtended($"Initial spawn delay: {initialSpawnDelay} seconds.");
        
        yield return new WaitForSeconds(initialSpawnDelay);

        Plugin.Instance.LogInfoExtended("Started awakening Nutcracker statues.");

        int amount = _nutcrackerObjects.Count;
        int spawnCount = 0;

        for (int i = 0; i < amount; i++)
        {
            if (!Plugin.ConfigManager.ToyStoreNutcracker_Enabled.Value)
            {
                Plugin.Instance.LogInfoExtended("Nutcracker spawning has been disabled in the config settings.");
                break;
            }

            int maxSpawnCount = Plugin.ConfigManager.ToyStoreNutcracker_MaxSpawnCount.Value;

            if (spawnCount >= maxSpawnCount)
            {
                Plugin.Instance.LogInfoExtended($"The max Nutcracker spawn count of {maxSpawnCount} has been reached.");
                break;
            }

            int index = GetNutcrackerStatueIndex();

            if (index < 0)
            {
                Plugin.logger.LogError($"Invalid Nutcracker statue index received.");
                break;
            }

            ActivateNutcrackerStatueOnServer(_nutcrackerObjects[index]);
            _nutcrackerObjects.RemoveAt(index);

            spawnCount++;

            float minSpawnDelay = Plugin.ConfigManager.ToyStoreNutcracker_MinSpawnDelay.Value;
            float maxSpawnDelay = Plugin.ConfigManager.ToyStoreNutcracker_MaxSpawnDelay.Value;
            float spawnDelay = Mathf.Max(Random.Range(minSpawnDelay, maxSpawnDelay), 0.25f);
            
            yield return new WaitForSeconds(spawnDelay);
        }

        Plugin.logger.LogInfo($"Nutcracker spawn coroutine has ended. Spawned {spawnCount}/{amount} Nutcrackers.");
    }

    private static int GetNutcrackerStatueIndex()
    {
        if (_nutcrackerObjects == null || _nutcrackerObjects.Count == 0)
        {
            return -1;
        }

        if (Plugin.ConfigManager.ToyStoreNutcracker_SpawnNearPlayers.Value)
        {
            PlayerControllerB playerScript = PlayerUtils.GetRandomPlayerScript();

            if (playerScript == null)
            {
                return GetRandomNutcrackerStatueIndex();
            }

            return GetNutcrackerStatueIndexNearPosition(playerScript.transform.position);
        }

        return GetRandomNutcrackerStatueIndex();
    }

    private static int GetRandomNutcrackerStatueIndex()
    {
        if (_nutcrackerObjects == null || _nutcrackerObjects.Count == 0)
        {
            return -1;
        }

        return Random.Range(0, _nutcrackerObjects.Count);
    }

    private static int GetNutcrackerStatueIndexNearPosition(Vector3 position)
    {
        if (_nutcrackerObjects == null || _nutcrackerObjects.Count == 0)
        {
            return -1;
        }

        int closestIndex = -1;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < _nutcrackerObjects.Count; i++)
        {
            if (_nutcrackerObjects[i] == null) continue;

            float distance = Vector3.Distance(_nutcrackerObjects[i].transform.position, position);

            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }

        return closestIndex;
    }

    private static void ActivateNutcrackerStatueOnServer(GameObject nutcrackerObject)
    {
        if (!NetworkUtils.IsServer) return;

        PluginNetworkBehaviour.Instance.HideNutcrackerStatueClientRpc(nutcrackerObject.transform.position);
        HideNutcrackerStatueOnLocalClient(nutcrackerObject.transform.position);

        Vector3 position = nutcrackerObject.transform.position;
        float yRot = nutcrackerObject.transform.eulerAngles.y;

        EnemyHelper.SpawnNutcrackerOnServer(position, yRot);
    }

    public static void HideNutcrackerStatueOnLocalClient(Vector3 position)
    {
        GameObject nutcrackerObject = null;

        foreach (var item in _nutcrackerObjects)
        {
            if (Vector3.Distance(item.transform.position, position) <= 0.25f)
            {
                nutcrackerObject = item;
                break;
            }
        }

        if (nutcrackerObject == null)
        {
            Plugin.logger.LogError($"Failed to hide Nutcracker statue. Could not find Nutcracker statue at position: ({position.x}, {position.y}, {position.z})");
            return;
        }

        nutcrackerObject.SetActive(false);
    }

    private static List<GameObject> GetNutcrackerStatues()
    {
        List<GameObject> nutcrackerObjects = [];

        foreach (var tileObject in Object.FindObjectsByType<DunGen.Tile>(FindObjectsSortMode.None).Select(_ => _.gameObject))
        {
            nutcrackerObjects.AddRange(GetNutcrackerStatuesInTile(tileObject));
        }

        return nutcrackerObjects;
    }

    private static List<GameObject> GetNutcrackerStatuesInTile(GameObject tileObject)
    {
        List<GameObject> nutcrackerObjects = [];

        foreach (var gameObject in tileObject.GetComponentsInChildren<LODGroup>().Select(_ => _.gameObject))
        {
            if (!gameObject.name.Contains("LOD0")) continue;

            GameObject parentObject = gameObject.transform.parent.gameObject;
            if (!parentObject.name.Contains("LOD0")) continue;
            
            if (nutcrackerObjects.Contains(parentObject) || _nutcrackerObjects.Contains(parentObject))
            {
                continue;
            }

            if (parentObject.transform.localScale != new Vector3(0.3f, 0.3f, 0.3f))
            {
                continue;
            }

            nutcrackerObjects.Add(parentObject);
        }

        return nutcrackerObjects;
    }
}
