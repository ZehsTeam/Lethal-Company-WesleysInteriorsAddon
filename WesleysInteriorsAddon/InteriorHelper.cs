using LethalLevelLoader;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal static class InteriorHelper
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
            Plugin.logger.LogError("Failed to get dungeon name.");
            return false;
        }
    }

    public static bool IsToyStoreInterior()
    {
        string dungeonName = GetDungeonName();

        if (dungeonName.Equals("ToystoreFlow", System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (dungeonName.Equals("Toy Store", System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
