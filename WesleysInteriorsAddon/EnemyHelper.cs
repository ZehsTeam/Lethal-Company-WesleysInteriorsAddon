using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal static class EnemyHelper
{
    public static void SpawnNutcrackerOnServer(Vector3 position, float yRot)
    {
        if (!NetworkUtils.IsServer)
        {
            Plugin.logger.LogError("Only the host can spawn enemies.");
            return;
        }

        SpawnEnemyOnServer("Nutcracker", position, yRot);
    }

    public static void SpawnEnemyOnServer(string enemyName, Vector3 position, float yRot)
    {
        if (!NetworkUtils.IsServer)
        {
            Plugin.logger.LogError("Only the host can spawn enemies.");
            return;
        }

        EnemyType enemyType = GetEnemyType(enemyName);

        if (enemyType == null)
        {
            Plugin.logger.LogError($"Failed to find EnemyType \"{enemyName}\".");
            return;
        }

        RoundManager.Instance.SpawnEnemyGameObject(position, yRot, -1, enemyType);

        Plugin.Instance.LogInfoExtended($"Spawned \"{enemyType.enemyName}\" at position: ({position.x}, {position.y}, {position.z}), yRot: {yRot}");
    }

    public static EnemyType GetEnemyType(string enemyName)
    {
        foreach (var enemyType in GetEnemyTypes())
        {
            if (enemyType.enemyName == enemyName)
            {
                return enemyType;
            }
        }

        try
        {
            EnemyType enemyType = Resources.FindObjectsOfTypeAll<EnemyType>().Single((EnemyType x) => x.enemyName == enemyName);

            if (IsValidEnemyType(enemyType) && NetworkUtils.IsNetworkPrefab(enemyType.enemyPrefab))
            {
                Plugin.Instance.LogInfoExtended($"Found EnemyType \"{enemyType.enemyName}\" from Resources.");

                return enemyType;
            }
        }
        catch { }

        return null;
    }

    public static List<EnemyType> GetEnemyTypes()
    {
        var enemyTypes = new HashSet<EnemyType>(new EnemyTypeComparer());

        foreach (var level in StartOfRound.Instance.levels)
        {
            var levelEnemyTypes = level.Enemies
                .Concat(level.DaytimeEnemies)
                .Concat(level.OutsideEnemies)
                .Select(e => e.enemyType)
                .Where(IsValidEnemyType);

            foreach (var levelEnemyType in levelEnemyTypes)
            {
                enemyTypes.Add(levelEnemyType);
            }
        }

        return enemyTypes.ToList();
    }

    public static bool IsValidEnemyType(EnemyType enemyType)
    {
        if (enemyType == null) return false;
        if (string.IsNullOrWhiteSpace(enemyType.enemyName)) return false;
        if (enemyType.enemyPrefab == null) return false;

        return true;
    }
}

public class EnemyTypeComparer : IEqualityComparer<EnemyType>
{
    public bool Equals(EnemyType x, EnemyType y)
    {
        if (x == null || y == null) return false;
        return x.enemyName == y.enemyName;
    }

    public int GetHashCode(EnemyType obj)
    {
        return obj.enemyName?.GetHashCode() ?? 0;
    }
}
