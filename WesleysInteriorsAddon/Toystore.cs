using com.github.zehsteam.WesleysInteriorsAddon.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal class Toystore
{
    private static List<GameObject> nutcrackerObjects = [];
    private static Coroutine activateNutcrackerStatuesCoroutine = null;

    public static void Reset()
    {
        nutcrackerObjects = [];

        if (activateNutcrackerStatuesCoroutine != null)
        {
            StartOfRound.Instance.StopCoroutine(activateNutcrackerStatuesCoroutine);
        }
    }

    public static void FindNutcrackerStatues()
    {
        nutcrackerObjects = GetNutcrackerStatues();

        Plugin.logger.LogInfo($"[Toystore] Found {nutcrackerObjects.Count} Nutcracker statues.");
    }

    public static void OnApparatusRemoved()
    {
        if (!Plugin.IsHostOrServer) return;
        if (!InteriorHelper.IsToystoreInterior()) return;

        Plugin.Instance.LogInfoExtended("[Toystore] Apparatus was pulled.");

        activateNutcrackerStatuesCoroutine = StartOfRound.Instance.StartCoroutine(ActivateNutcrackerStatuesOnServer());
    }

    private static IEnumerator ActivateNutcrackerStatuesOnServer()
    {
        if (!Plugin.IsHostOrServer) yield break;
        if (nutcrackerObjects.Count == 0) yield break;

        float minInitialActivateTime = Plugin.ConfigManager.ToystoreNutcrackerMinInitialActivateTime;
        float maxInitialActivateTime = Plugin.ConfigManager.ToystoreNutcrackerMaxInitialActivateTime;
        float minActivateTime = Plugin.ConfigManager.ToystoreNutcrackerMinActivateTime;
        float maxActivateTime = Plugin.ConfigManager.ToystoreNutcrackerMaxActivateTime;

        yield return new WaitForSeconds(Random.Range(minInitialActivateTime, maxInitialActivateTime + 1f));

        Plugin.Instance.LogInfoExtended("[Toystore] Start activating Nutcrackers.");

        int amount = nutcrackerObjects.Count;

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, nutcrackerObjects.Count);
            ActivateNutcrackerStatueOnServer(nutcrackerObjects[index]);
            nutcrackerObjects.RemoveAt(index);

            yield return new WaitForSeconds(Random.Range(minActivateTime, maxActivateTime + 1f));
        }
    }

    private static void ActivateNutcrackerStatueOnServer(GameObject nutcrackerObject)
    {
        if (!Plugin.IsHostOrServer) return;

        PluginNetworkBehaviour.Instance.HideNutcrackerStatueClientRpc(nutcrackerObject.transform.position);
        HideNutcrackerStatueOnLocalClient(nutcrackerObject.transform.position);

        Vector3 position = nutcrackerObject.transform.position;
        float yRot = nutcrackerObject.transform.eulerAngles.y;

        EnemyHelper.SpawnNutcrackerOnServer(position, yRot);
    }

    public static void HideNutcrackerStatueOnLocalClient(Vector3 position)
    {
        GameObject nutcrackerObject = null;

        foreach (var item in nutcrackerObjects)
        {
            if (Vector3.Distance(item.transform.position, position) <= 0.25f)
            {
                nutcrackerObject = item;
                break;
            }
        }

        if (nutcrackerObject == null)
        {
            Plugin.logger.LogError($"[Toystore] Failed to hide Nutcracker statue. Could not find Nutcracker statue at position: ({position.x}, {position.y}, {position.z})");
            return;
        }

        HideNutcrackerStatueOnLocalClient(nutcrackerObject);
    }

    public static void HideNutcrackerStatueOnLocalClient(GameObject nutcrackerObject)
    {
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

        foreach (var item in tileObject.GetComponentsInChildren<LODGroup>().Select(_ => _.transform))
        {
            if (!item.name.Contains("LOD0")) continue;
            if (item.localScale != new Vector3(0.3f, 0.3f, 0.3f)) continue;

            nutcrackerObjects.Add(item.gameObject);
        }

        return nutcrackerObjects;
    }
}
