using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal class EnemyHelper
{
    public static void SpawnNutcrackerOnServer(Vector3 position, float yRot)
    {
        SpawnEnemyOnServer("Nutcracker", position, yRot);
    }

    public static void SpawnEnemyOnServer(string enemyName, Vector3 position, float yRot)
    {
        EnemyType enemyType = GetEnemyType(enemyName);

        if (enemyType == null)
        {
            Plugin.logger.LogError($"[EnemyHelper] Failed to find EnemyType \"{enemyName}\".");
            return;
        }

        RoundManager.Instance.SpawnEnemyGameObject(position, yRot, -1, enemyType);

        Plugin.Instance.LogInfoExtended($"[EnemyHelper] Spawned \"{enemyType.enemyName}\" at position: ({position.x}, {position.y}, {position.z}), yRot: {yRot}");
    }

    public static EnemyType GetEnemyType(string enemyName)
    {
        List<EnemyType> enemyTypes = GetAllEnemyTypes();

        foreach (var enemyType in enemyTypes)
        {
            if (enemyType.enemyName == enemyName)
            {
                return enemyType;
            }
        }

        return null;
    }

    private static List<EnemyType> GetAllEnemyTypes()
    {
        List<EnemyType> enemyTypes = [];

        foreach (var level in StartOfRound.Instance.levels)
        {
            enemyTypes.AddRange(level.Enemies.Select(_ => _.enemyType));
            enemyTypes.AddRange(level.DaytimeEnemies.Select(_ => _.enemyType));
            enemyTypes.AddRange(level.OutsideEnemies.Select(_ => _.enemyType));
        }

        enemyTypes = enemyTypes.Distinct().ToList();

        return enemyTypes;
    }
}
