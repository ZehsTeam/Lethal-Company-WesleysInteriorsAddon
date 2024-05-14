using LethalLevelLoader;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal class InteriorHelper
{
    public static string GetDungeonName()
    {
        return TryGetDungeonName(out string dungeonName) ? dungeonName : string.Empty;
    }

    public static bool TryGetDungeonName(out string dungeonName)
    {
        dungeonName = string.Empty;

        try
        {
            ExtendedDungeonFlow extendedDungeonFlow = DungeonManager.CurrentExtendedDungeonFlow;
            if (extendedDungeonFlow == null) return false;

            dungeonName = extendedDungeonFlow.DungeonName;
            return true;
        }
        catch
        {
            Plugin.logger.LogError("[InteriorHelper] Failed to get dungeon name.");
            return false;
        }
    }

    public static bool IsToystoreInterior()
    {
        return GetDungeonName() == "ToystoreFlow";
    }
}
